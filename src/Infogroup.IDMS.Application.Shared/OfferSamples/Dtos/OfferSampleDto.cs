using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.OfferSamples.Dtos
{
    public class OfferSampleDto : EntityDto
    {
		public string cDescription { get; set; }

		public int OfferId { get; set; }

        public string cFileName { get; set; }

    }
}