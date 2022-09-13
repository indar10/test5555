using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.SegmentLists.Dtos;

namespace Infogroup.IDMS.SegmentLists
{
    public interface ISegmentListsAppService : IApplicationService 
    {
        Task<List<SourceDto>> FetchApprovedSources(GetAllApprovedSourcesInput input);
        GetExistingSourceDataForView GetExistingSourceData(int iSegmentID);
        Task SaveSources(SaveSourcesInputDto input);
    }
}