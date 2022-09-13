using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Offers.Dtos
{
    public class OfferDto : EntityDto
    {
		public string cOfferCode { get; set; }

		public string cOfferName { get; set; }

		public string LK_OfferType { get; set; }

		public bool iIsActive { get; set; }


		 public int MailerId { get; set; }

		 
    }
}