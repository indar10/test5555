
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.DivisionShipTos.Dtos
{
    public class DivisionShipToDto : EntityDto
    {
		public string cCode { get; set; }

		public string cCompany { get; set; }

		public string cFirstName { get; set; }

		public string cLastName { get; set; }

		public string cCountry { get; set; }

		public string cPhone { get; set; }

		public string cEmail { get; set; }

        public string DivisionName { get; set; }

    }
}