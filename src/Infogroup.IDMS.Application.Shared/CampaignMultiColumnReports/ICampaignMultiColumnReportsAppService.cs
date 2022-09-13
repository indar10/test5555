using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.CampaignMultiColumnReports
{
    public interface ICampaignMultiColumnReportsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCampaignMultiColumnReportForViewDto>> GetAll(GetAllCampaignMultiColumnReportsInput input);

		Task<GetCampaignMultiColumnReportForEditOutput> GetCampaignMultiColumnReportForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCampaignMultiColumnReportDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<CampaignMultiColumnReportCampaignLookupTableDto>> GetAllCampaignForLookupTable(GetAllForLookupTableInput input);
		
    }
}