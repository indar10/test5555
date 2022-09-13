using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infogroup.IDMS.SavedSelectionDetails.Dtos;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Infogroup.IDMS.SavedSelectionsDetails;
using Abp.UI;

namespace Infogroup.IDMS.SavedSelectionDetails
{
    public class SavedSelectionDetailsAppService : IDMSAppServiceBase, ISavedSelectionDetailsAppService
    {
        private readonly SavedSelectionDetailRepository _customSavedSelectionDetailRepository;

        public SavedSelectionDetailsAppService(SavedSelectionDetailRepository customSavedSelectionDetailRepository) 
		{
            _customSavedSelectionDetailRepository = customSavedSelectionDetailRepository;

        }

		public async Task<List<GetSavedSelectionDetailForViewDto>> GetAllSavedSelectionsDetails(GetAllSavedSelectionDetailsInput input)
        {
            try
            {
                return await _customSavedSelectionDetailRepository.GetAllSavedSelectionDetail(input);                 
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
		 
		 
    }
}