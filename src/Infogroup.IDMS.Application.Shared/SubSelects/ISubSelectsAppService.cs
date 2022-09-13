using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.SubSelects.Dtos;

namespace Infogroup.IDMS.SubSelects
{
    public interface ISubSelectsAppService : IApplicationService 
    {
        Task<List<SubSelectForViewDto>> GetAllSubSelect(int segmentId);
        Task<int> CreateSubSelect(CreateOrEditSubSelectDto input);
        void UpdateSubSelect(CreateOrEditSubSelectDto input);

        Task DeleteSubSelect(int campaignId, int subSelectId);
    }
}