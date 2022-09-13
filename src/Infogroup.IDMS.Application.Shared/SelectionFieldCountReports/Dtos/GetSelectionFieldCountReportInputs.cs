using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SelectionFieldCountReports.Dtos
{
    public class GetSelectionFieldCountReportInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int SelectedDatabase { get; set; }
        public string SelectedcQuestionFieldName { get; set; }
        public string SelectediStatus { get; set; }
        public bool DownloadFlag = true;
    }
}
