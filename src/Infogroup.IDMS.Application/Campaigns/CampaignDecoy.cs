using Infogroup.IDMS.Decoys.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.CampaignDecoys;
using System.Diagnostics;
using Abp.UI;

namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region Fetch Decoy
        private GetDecoyForViewDto FetchDecoys(Campaign campaign)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var fetchDecoys = new GetDecoyForViewDto
                {
                    listOfDecoys = GetAllDecoysList(campaign.Id),
                    isDecoyKeyMethod = campaign.iDecoyKeyMethod,
                    decoyKey = campaign.cDecoyKey.Trim(),
                    decoyKey1 = campaign.cDecoyKey1.Trim(),
                    listOfDecoyGroup = GetDecoyGroup(campaign),
                    listOfGroupsForEdit = FillGroupsForEdit()
                };
                sw.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaign.Id}, Total time for FetchDecoys: {sw.Elapsed.TotalSeconds} ----- \r\n");
                return fetchDecoys;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public List<DropdownOutputDto> GetDecoyGroup(Campaign campaign)
        {
            var listOfDecoyGroups = (from decoy in _decoyRepository.GetAll()
                                        //join campaign in _campaignRepository.GetAll()
                                        //on decoy.MailerID equals campaign.MailerID
                                        where decoy.MailerID == campaign.MailerID && !string.IsNullOrEmpty(decoy.cDecoyGroup)
                                        orderby decoy.cDecoyGroup select new DropdownOutputDto { Label= decoy.cDecoyGroup,Value= decoy.cDecoyGroup } ).Distinct().ToList();
            return listOfDecoyGroups;
        }
        public List<DecoyDto> GetDecoyGroupList(int campaignID ,string cDecoyGroup)
        {
            var listOfDecoyGroups = (from decoy in _decoyRepository.GetAll()
                                     join campaign in _campaignRepository.GetAll()
                                     on decoy.MailerID equals campaign.MailerID
                                     where campaign.Id.Equals(campaignID) && decoy.cDecoyGroup== cDecoyGroup
                                     select new DecoyDto
                                     { 
                                         cName = decoy.cName.Trim(),
                                         cAddress1 = decoy.cAddress1,
                                         cAddress2 = decoy.cAddress2.Trim(),
                                         cDecoyGroup = decoy.cDecoyGroup.Trim(),
                                         cCompany = decoy.cCompany.Trim(),
                                         cDecoyType = decoy.cDecoyType,
                                         cZip = decoy.cZip.Trim(),
                                         cZip4 = decoy.cZip4.Trim(),
                                         cPhone = decoy.cPhone,
                                         cFax = decoy.cFax.Trim(),
                                         cEmail = decoy.cEmail.Trim(),
                                         cKeyCode1 = decoy.cKeyCode1.Trim(),
                                         cAddress = $"{decoy.cCity.Trim()},{decoy.cState.Trim()} {decoy.cZip.Trim()}{(string.IsNullOrEmpty(decoy.cZip4.Trim()) || decoy.cZip4.Trim().Equals("0") ? string.Empty : $"-{decoy.cZip4.Trim()}")}",
                                         Action = ActionType.Add,
                                         cFirstName = decoy.cFirstName.Trim(),
                                         cLastName = decoy.cLastName.Trim(),
                                         cState = decoy.cState.Trim(),
                                         cCity = decoy.cCity.Trim(),
                                         cTitle = decoy.cTitle.Trim(),
                                         isDecoyGroupType = decoy.cDecoyType.Equals("G", StringComparison.OrdinalIgnoreCase)
                                     }).ToList();
            return ObjectMapper.Map<List<DecoyDto>>(listOfDecoyGroups);
        }
        private static List<DropdownOutputDto> FillGroupsForEdit()
        {
            var listOfGroupsKey = new List<DropdownOutputDto>();
            for (int i = 65; i < 91; i++)
            {
                var key=Convert.ToChar(i).ToString();
                listOfGroupsKey.Add(new DropdownOutputDto { Value = key, Label = key }
                );
            }
            return listOfGroupsKey;
        }

        private List<DecoyDto> GetAllDecoysList(int camapignID)
        {
            var lstOfDecoy = (from decoy in _campaignDecoyRepository.GetAll()
                              where decoy.OrderId.Equals(camapignID)
                              select new DecoyDto
                              {
                                  Id = decoy.Id,
                                  cName = decoy.cName.Trim(),
                                  cAddress1 = decoy.cAddress1,
                                  cAddress2 = decoy.cAddress2.Trim(),
                                  cDecoyGroup = decoy.cDecoyGroup.Trim(),
                                  cCompany = decoy.cCompany.Trim(),
                                  cDecoyType = decoy.cDecoyType,
                                  cZip = decoy.cZip.Trim(),
                                  cZip4 = decoy.cZip4.Trim(),
                                  cPhone = decoy.cPhone,
                                  cFax = decoy.cFax.Trim(),
                                  cEmail = decoy.cEmail.Trim(),
                                  cKeyCode1 = decoy.cKeyCode1.Trim(),
                                  cAddress = $"{decoy.cCity.Trim()},{decoy.cState.Trim()} {decoy.cZip.Trim()}{(string.IsNullOrEmpty(decoy.cZip4.Trim()) || decoy.cZip4.Trim().Equals("0") ? string.Empty : $"-{decoy.cZip4.Trim()}")}",
                                  Action = ActionType.None,
                                  cFirstName = decoy.cFirstName.Trim(),
                                  cLastName = decoy.cLastName.Trim(),
                                  cState = decoy.cState.Trim(),
                                  cCity = decoy.cCity.Trim(),
                                  cTitle = decoy.cTitle.Trim(),
                                  isDecoyGroupType = decoy.cDecoyType.Equals("G", StringComparison.OrdinalIgnoreCase)
                              }).ToList();
            return lstOfDecoy;
        }

        #endregion

        #region Save Decoys

        private void SaveDecoys(GetDecoyForViewDto createEditDecoy,int campaignId)
        {
            var listOfDecoys = createEditDecoy.listOfDecoys.Where(t => t.Action!=ActionType.None || (t.Action == ActionType.Delete && t.Id != 0)).ToList();
            foreach (var decoy in listOfDecoys)
            {
                var decoyData = CommonHelpers.ConvertNullStringToEmptyAndTrim(decoy);
                switch (decoy.Action)
                {
                    case ActionType.Add:
                        {
                            //TODO: Will map below object with objectmapper at the time of refactoring
                            var CampaignDecoy = new CampaignDecoy
                            {
                                cAddress1 = decoyData.cAddress1,
                                cAddress2 = decoyData.cAddress2,
                                cZip = decoyData.cZip,
                                cZip4 = decoyData.cZip4,
                                cPhone = decoyData.cPhone,
                                cFax = decoyData.cFax,
                                cKeyCode1 = decoyData.cKeyCode1,
                                cEmail = decoyData.cEmail,
                                cFirstName = decoyData.cFirstName,
                                cLastName = decoyData.cLastName,
                                cName = decoyData.cName,
                                cCompany = decoyData.cCompany,
                                cTitle = decoyData.cTitle,
                                cState = decoyData.cState,
                                cCity = decoyData.cCity,
                                cDecoyGroup = decoyData.cDecoyGroup,
                                cDecoyType = decoyData.cDecoyType,
                                OrderId = campaignId,
                                cCreatedBy = _mySession.IDMSUserName,
                                dCreatedDate = DateTime.Now
                            };
                            _campaignDecoyRepository.Insert(CampaignDecoy);
                            CurrentUnitOfWork.SaveChanges();
                            break;
                        }
                    case ActionType.Delete:
                        {
                            _campaignDecoyRepository.Delete(decoyData.Id);
                            break;
                        }
                    case ActionType.Edit:
                        {
                            //TODO: Will map below object with objectmapper at the time of refactoring
                            var updateDecoy = _campaignDecoyRepository.Get(decoyData.Id);
                            updateDecoy.cAddress1 = decoyData.cAddress1;
                            updateDecoy.cAddress2 = decoyData.cAddress2;
                            updateDecoy.cZip = decoyData.cZip;
                            updateDecoy.cZip4 = decoyData.cZip4;
                            updateDecoy.cFirstName = decoyData.cFirstName;
                            updateDecoy.cLastName = decoyData.cLastName;
                            updateDecoy.cName = decoyData.cName;
                            updateDecoy.cTitle = decoyData.cTitle;
                            updateDecoy.cCompany = decoyData.cCompany;
                            updateDecoy.cDecoyGroup = decoyData.cDecoyGroup;
                            updateDecoy.cEmail = decoyData.cEmail;
                            updateDecoy.cCity = decoyData.cCity;
                            updateDecoy.cKeyCode1 = decoyData.cKeyCode1;
                            updateDecoy.cFax = decoyData.cFax;
                            updateDecoy.cPhone = decoyData.cPhone;
                            updateDecoy.cDecoyType = decoyData.cDecoyType;
                            updateDecoy.dModifiedDate = DateTime.Now;
                            updateDecoy.cModifiedBy = _mySession.IDMSUserName;
                            _campaignDecoyRepository.Update(updateDecoy);
                            CurrentUnitOfWork.SaveChanges();
                            break;
                        }
                    default:
                        throw new Exception("Unexpected Case");
                }
            }
        }
        #endregion

        #region Validations
        public bool ValidateDecoys(DecoyDto decoysRecord)
        {
            var result = false;
            var isFirstName = !decoysRecord.cFirstName.Trim().Contains("<##DK##>");
            var isLastName = !decoysRecord.cLastName.Trim().Contains("<##DK##>");
            var isAddress1= !decoysRecord.cAddress1.Trim().Contains("<##DK##>");
            var isKeyCode1 = decoysRecord.cKeyCode1!=null?!decoysRecord.cKeyCode1.Trim().Contains("<##DK##>"):true;

            if (isFirstName && isLastName && isAddress1 && isKeyCode1)
                result = true;
            else
                result = false;

            return result;   
        }

        #endregion
    }
}
