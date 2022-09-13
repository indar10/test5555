using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ExportLayoutDetails.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.ExportLayoutDetails
{
    public interface IExportLayoutDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetExportLayoutDetailForViewDto>> GetAll(GetAllExportLayoutDetailsInput input);

		Task<GetExportLayoutDetailForEditOutput> GetExportLayoutDetailForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditExportLayoutDetailDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<ExportLayoutDetailExportLayoutLookupTableDto>> GetAllExportLayoutForLookupTable(GetAllForLookupTableInput input);
		
    }
}