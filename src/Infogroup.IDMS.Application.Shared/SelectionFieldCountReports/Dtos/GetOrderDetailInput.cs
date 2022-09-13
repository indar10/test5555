using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SelectionFieldCountReports.Dtos
{
   public class GetOrderDetailInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public string cQuestionFieldName { get; set; }
        public int iStatus { get; set; }
        public int SelectedDatabase { get; set; }
        public bool DownloadFlag = true;
    }
}
