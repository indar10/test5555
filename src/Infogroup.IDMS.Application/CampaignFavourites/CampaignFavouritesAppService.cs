using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignFavourites.Dtos;
using Abp.Authorization;
using Infogroup.IDMS.Sessions;
using System;
using Abp.UI;
using Infogroup.IDMS.IDMSUsers;
using System.Collections.Generic;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Databases;

namespace Infogroup.IDMS.CampaignFavourites
{
    [AbpAuthorize]
    public class CampaignFavouritesAppService : IDMSAppServiceBase, ICampaignFavouritesAppService
    {
        private readonly IRepository<CampaignFavourite, int> _campaignFavouriteRepository;
        private readonly AppSession _mySession;
        private readonly IRedisIDMSUserCache _userCache;

        public CampaignFavouritesAppService(
            IRepository<CampaignFavourite> campaignFavouriteRepository,
            AppSession mySession,
            IRedisIDMSUserCache userCache)
        {
            _campaignFavouriteRepository = campaignFavouriteRepository;
            _mySession = mySession;
            _userCache = userCache;

        }

        public List<CampaignFavouriteDtoForView> GetAllFavouriteCampaigns()
        {
            try
            {
                var favouriteCampaigns=  _userCache.GetCampaignFavourites(_mySession.IDMSUserId);
                favouriteCampaigns.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.DatabaseName) && (p.DatabaseName.ToLower().StartsWith(DatabaseNameConst.Infogroup) || p.DatabaseName.ToLower().EndsWith(DatabaseNameConst.Database)))
                    {
                        p.DatabaseName = p.DatabaseName.Replace(DatabaseNameConst.Database, "", StringComparison.OrdinalIgnoreCase);
                        p.DatabaseName = p.DatabaseName.Replace(DatabaseNameConst.Infogroup, "", StringComparison.OrdinalIgnoreCase);
                    }
                });
                return favouriteCampaigns;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void AddOrRemoveFavouriteCampaigns(int campaignId, bool isFavourite)
        {
            try
            {
                if (!isFavourite)
                {
                    var newFavourite = new CampaignFavourite
                    {
                        OrderID = campaignId,
                        UserID = _mySession.IDMSUserId,
                        cCreatedBy = _mySession.IDMSUserName,
                        dCreatedDate = DateTime.Now
                    };
                    _campaignFavouriteRepository.Insert(newFavourite);
                }
                else
                {
                    _campaignFavouriteRepository.Delete(p => p.OrderID == campaignId && p.UserID == _mySession.IDMSUserId);

                }
                CurrentUnitOfWork.SaveChanges();
                _userCache.SetCampaignFavourites(_mySession.IDMSUserId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }

}


