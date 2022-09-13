using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Reports.Dtos;


namespace Infogroup.IDMS.Reports

{
    public interface IReportsAppService : IApplicationService
    {
        Task<GetReportForViewDto> GetReports();

        FileDto GenerateExcelReport(Dictionary<string, Object> excelData);

        List<ReportDto> GetAllForReportsDropdown();

    }
}