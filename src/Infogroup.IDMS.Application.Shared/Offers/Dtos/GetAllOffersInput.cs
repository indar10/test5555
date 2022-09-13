using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Offers.Dtos
{
    public class GetAllOffersInput : PagedAndSortedResultRequestDto
    {
        public int MailerId { get; set; }

    }
}