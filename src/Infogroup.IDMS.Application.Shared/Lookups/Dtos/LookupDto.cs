
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Lookups.Dtos
{
    public class LookupDto : EntityDto
    {
		public int ID { get; set; }

		public string cLookupValue { get; set; }

		public int iOrderBy { get; set; }

		public string cCode { get; set; }

		public string cDescription { get; set; }

		public string cField { get; set; }

		public string mField { get; set; }

		public int iField { get; set; }

		public bool iIsActive { get; set; }

		public string cCreatedBy { get; set; }

		public string cModifiedBy { get; set; }

		public DateTime dCreatedDate { get; set; }

		public DateTime? dModifiedDate { get; set; }



    }
}