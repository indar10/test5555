using Abp.Application.Services;
using Infogroup.IDMS.SICCodes.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.SICCodes
{
    public interface ISICCodesAppService : IApplicationService 
    {
        List<TreeNode> GetSICCode(GetAllSICCodesInput input);
        FranchiseNIndusdustry GetFranchiseNIndustryCode(string cSICCode, string cIndicator);
    }
}