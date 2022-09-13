using Abp.Application.Services.Dto;
namespace Infogroup.IDMS.SubSelects.Dtos
{
    public class GetAllSubSelectsInput : PagedAndSortedResultRequestDto
    {
		public int SegmentId { get; set; }		 
    }
}