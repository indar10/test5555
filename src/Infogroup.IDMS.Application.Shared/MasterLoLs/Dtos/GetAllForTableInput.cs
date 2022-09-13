using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MasterLoLs.Dtos
{
    public class GetAllForTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        //public int iIsActiveFilter { get; set; }
        public int SelectedDatabase { get; set; }
    }
}