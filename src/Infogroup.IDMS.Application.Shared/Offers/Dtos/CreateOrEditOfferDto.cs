using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Offers.Dtos
{
    public class CreateOrEditOfferDto : EntityDto<int?>
    {

		public string cOfferCode { get; set; }		
		
		[Required]
		public string cOfferName { get; set; }		
		
		[Required]
		public string LK_OfferType { get; set; }

        public bool iIsActive { get; set; }		
		
		public bool iHideInDWAP { get; set; }	
        
		public bool isAutoApprove { get; set; }

        public int MailerId { get; set; }

        public string cCreatedBy { get; set; }

        public string cModifiedBy { get; set; }

        public DateTime dCreatedDate { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public List<DropdownOutputDto> OfferTypeDescription;

    }
}