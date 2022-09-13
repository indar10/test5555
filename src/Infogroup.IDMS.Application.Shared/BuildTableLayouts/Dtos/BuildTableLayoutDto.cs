
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.BuildTableLayouts.Dtos
{
    public class BuildTableLayoutDto : EntityDto
    {
		public string cFieldName { get; set; }

		public string cFieldDescription { get; set; }

		public string cDataType { get; set; }

		public int iDataLength { get; set; }

		public bool iIsMailerSpecific { get; set; }

		public bool iIsSystem { get; set; }

		public bool iIsListSpecific { get; set; }

		public bool iIsNormalized { get; set; }

		public bool iIsEnhancement { get; set; }

		public bool iIsMappingRequired { get; set; }

		public bool iIsMappingAllowed { get; set; }

		public string cNormFieldName { get; set; }

		public bool iAuditType { get; set; }

		public bool iIsSelectable { get; set; }

		public bool iIsBillable { get; set; }

		public bool iShowListBox { get; set; }

		public bool iShowTextBox { get; set; }

		public bool iFileOperations { get; set; }

		public bool iShowDefault { get; set; }

		public bool iAllowSorting { get; set; }

		public bool iAllowMaxPer { get; set; }

		public bool iLayoutOrder { get; set; }

		public bool iAllowExport { get; set; }

		public bool iDisplayOrder { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public DateTime? dModifiedDate { get; set; }

		public string cModifiedBy { get; set; }

		public string LK_IndexType { get; set; }

		public int? iKeyColumn { get; set; }

		public bool iIsRebuildDD { get; set; }

		public string cGroupName { get; set; }


		 public int BuildTableId { get; set; }

		 
    }
}