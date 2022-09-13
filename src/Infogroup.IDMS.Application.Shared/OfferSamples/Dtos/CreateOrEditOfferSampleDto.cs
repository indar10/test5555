using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.OfferSamples.Dtos
{
    public class CreateOrEditOfferSampleDto : EntityDto<int?>
    {

		public string cDescription { get; set; }
		public string cFileName { get; set; }
		public int OfferId { get; set; }
		public string MailerCompany { get; set; }
        public string cCreatedBy { get; set; }
        public DateTime dCreatedDate { get; set; }
        public string cModifiedBy { get; set; }
        public DateTime? dModifiedDate { get; set; }

    }
}