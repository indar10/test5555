using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignMaxPers.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.CampaignMaxPers
{
    public interface ICampaignMaxPersRepository: IRepository<CampaignMaxPer, int>
    {
        List<DropdownOutputDto> GetMaxPerFieldsDropdown(int buildId);
        int GetBuildLolByCampaignId(int iCampaignId);
        int GetOrderListCount(int iCampaignId);
        List<string> GetGroupsWithNotSegments(int iCampaignID);
        List<string> GetUnDefinedGroupsForSegments(int iCampaignID);

    }
}
