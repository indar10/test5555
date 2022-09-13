using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.UserSavedSelections.Dtos;
using Abp.Authorization;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.SegmentSelections;
using System;
using Infogroup.IDMS.SegmentSelections.Dtos;
using Abp.UI;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Infogroup.IDMS.UserSavedSelections
{
	[AbpAuthorize]
    public class UserSavedSelectionsAppService : IDMSAppServiceBase, IUserSavedSelectionsAppService
    {
		 private readonly IRepository<UserSavedSelection> _userSavedSelectionRepository;
         private readonly AppSession _mySession;
         private readonly ISegmentSelectionRepository _segmentSelectionRepository;

        public UserSavedSelectionsAppService(IRepository<UserSavedSelection> userSavedSelectionRepository ,
           ISegmentSelectionRepository segmentSelectionRepository,
           AppSession mySession) 
		  {
			_userSavedSelectionRepository = userSavedSelectionRepository;
            _segmentSelectionRepository = segmentSelectionRepository;
            _mySession = mySession;
          }
		
		 public async Task<string> Create(int sourceSegment, CreateOrEditUserSavedSelectionDto input)
         {
           try
            {
                var nonUniqueDescription = _userSavedSelectionRepository.GetAll()
                    .Any(userSelection =>
                      userSelection.iIsActive
                   && userSelection.UserID == _mySession.IDMSUserId
                   && userSelection.DatabaseId == input.DatabaseId
                   && userSelection.cChannelType == input.cChannelType
                   && userSelection.cDescription == input.cDescription);
                if (nonUniqueDescription)
                    throw new UserFriendlyException(L("NameNotUniqueMessage"));
                var userSavedSelection = ObjectMapper.Map<UserSavedSelection>(input);
                userSavedSelection.UserID = _mySession.IDMSUserId;
                userSavedSelection.cCreatedBy = _mySession.IDMSUserName;
                userSavedSelection.dCreatedDate = DateTime.Now;
                var userSavedSelectionId  = await _userSavedSelectionRepository.InsertAndGetIdAsync(userSavedSelection);               
                CurrentUnitOfWork.SaveChanges();
                // Removing defaults from other selections
                if (input.iIsDefault)
                {
                   _userSavedSelectionRepository.GetAll()
                    .Where(userSelection =>
                      userSelection.Id != userSavedSelectionId
                   && userSelection.iIsDefault
                   && userSelection.UserID == _mySession.IDMSUserId
                   && userSelection.DatabaseId == input.DatabaseId
                   && userSelection.cChannelType == input.cChannelType)
                   .ToList()
                   .ForEach(selection => selection.iIsDefault = false);
                    CurrentUnitOfWork.SaveChanges();
                }
                var bulkOperationsInput = new SaveGlobalChangesInputDto
                {
                    CampaignId = userSavedSelectionId,
                    UserID = _mySession.IDMSUserName,
                    TargetSegments = "AAAAAAA",
                    Action = GlobalChangesAction.ADD_TO_FAVORITES,
                    SourceSegment = sourceSegment
                };
                return await _segmentSelectionRepository.ApplyBulkOperations(bulkOperationsInput);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }		
    }
}