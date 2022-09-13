using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ShippedReports.Dtos
{
    public class GetShippedReportInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
