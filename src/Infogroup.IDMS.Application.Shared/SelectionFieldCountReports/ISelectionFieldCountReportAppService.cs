using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SelectionFieldCountReports.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SelectionFieldCountReports
{
    public interface ISelectionFieldCountReportAppService: IApplicationService
    {
        PagedResultDto<GetSelectionFieldCountReportView> GetAllSelectionFieldCountReports(GetSelectionFieldCountReportInput input);
        PagedResultDto<GetOrderDetailsView> GetOrderDetails(GetOrderDetailInput input);        
    }
}
