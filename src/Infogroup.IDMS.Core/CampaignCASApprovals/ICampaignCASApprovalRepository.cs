using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignCASApprovals.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.CampaignCASApprovals
{
    public interface ICampaignCASApprovalRepository : IRepository<CampaignCASApproval, int>
    {
         Task<List<CampaignCASApprovalDto>> getCASApprovalList(int? iOfferID, int BuildID);
    }
}
