using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Shared.Dtos
{
    public class GetAllSetupInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int iIsActiveFilter { get; set; }
        public int SelectedDatabase { get; set; }
        public string ContactLastNameFilterText { get; set; }
        public string ContactEmailFilterText { get; set; }
    }
}