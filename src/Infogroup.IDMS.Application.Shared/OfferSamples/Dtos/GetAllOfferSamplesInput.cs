using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.OfferSamples.Dtos
{
    public class GetAllOfferSamplesInput : PagedAndSortedResultRequestDto
    {
		public int OfferId { get; set; }

    }
}