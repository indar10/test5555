using System.Collections.Generic;
using Abp.Application.Services;
using Infogroup.IDMS.CampaignFavourites.Dtos;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.CampaignFavourites
{
    public interface ICampaignFavouritesAppService : IApplicationService 
    {
        List<CampaignFavouriteDtoForView> GetAllFavouriteCampaigns();
        void AddOrRemoveFavouriteCampaigns(int campaignId, bool isFavourite);
    }
}