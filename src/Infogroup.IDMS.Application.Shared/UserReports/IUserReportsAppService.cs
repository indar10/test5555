using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.UserReports.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.UserReports
{
    public interface IUserReportsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserReportForViewDto>> GetAll(GetAllUserReportsInput input);

		Task<GetUserReportForEditOutput> GetUserReportForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserReportDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<UserReportTblUserLookupTableDto>> GetAllTblUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<UserReportReportLookupTableDto>> GetAllReportForLookupTable(GetAllForLookupTableInput input);
		
    }
}