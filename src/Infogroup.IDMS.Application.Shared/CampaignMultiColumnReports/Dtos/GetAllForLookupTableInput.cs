using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.CampaignMultiColumnReports.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}