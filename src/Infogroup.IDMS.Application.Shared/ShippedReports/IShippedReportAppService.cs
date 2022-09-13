using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ShippedReports.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.ShippedReports
{
    public interface IShippedReportAppService: IApplicationService
    {
        PagedResultDto<GetShippedReportView> GetAllShippedReports(GetShippedReportInput input);
    }
}
