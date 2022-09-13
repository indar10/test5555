using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Brokers.Dtos
{


    public class GetAllBrokersInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int iIsActiveFilter { get; set; }
        public int SelectedDatabase { get; set; }
        public string ContactLastNameFilterText { get; set; }
        public string ContactEmailFilterText { get; set; }
    }
}
