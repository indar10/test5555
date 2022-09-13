
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class SegmentDto : EntityDto
    {
		public string cDescription { get; set; }

		public bool iUseAutosuppress { get; set; }

		public string cKeyCode1 { get; set; }

		public string cKeyCode2 { get; set; }

		public int iRequiredQty { get; set; }

		public int iProvidedQty { get; set; }

		public int iDedupeOrderSpecified { get; set; }

		public int iDedupeOrderUsed { get; set; }

		public string cMaxPerGroup { get; set; }

		public string cTitleSuppression { get; set; }

		public string cFixedTitle { get; set; }

		public int iDoubleMultiBuyerCount { get; set; }

		public bool iIsOrderLevel { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public DateTime dModifiedDate { get; set; }

		public string cModifiedBy { get; set; }

		public int? iGroup { get; set; }

		public int? iOutputQty { get; set; }

		public int? iAvailableQty { get; set; }

		public bool iIsRandomRadiusNth { get; set; }

		public int OrderId { get; set; }

		 
    }
}