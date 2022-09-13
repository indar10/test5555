
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.BuildTableLayouts.Dtos
{
    public class CreateOrEditBuildTableLayoutDto : EntityDto<int?>
    {

		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcFieldNameLength, MinimumLength = BuildTableLayoutConsts.MincFieldNameLength)]
		public string cFieldName { get; set; }
		
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcFieldDescriptionLength, MinimumLength = BuildTableLayoutConsts.MincFieldDescriptionLength)]
		public string cFieldDescription { get; set; }
		
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcDataTypeLength, MinimumLength = BuildTableLayoutConsts.MincDataTypeLength)]
		public string cDataType { get; set; }
		
		
		public int iDataLength { get; set; }
		
		
		public bool iIsMailerSpecific { get; set; }
		
		
		public bool iIsSystem { get; set; }
		
		
		public bool iIsListSpecific { get; set; }
		
		
		public bool iIsNormalized { get; set; }
		
		
		public bool iIsEnhancement { get; set; }
		
		
		public bool iIsMappingRequired { get; set; }
		
		
		public bool iIsMappingAllowed { get; set; }
		
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcNormFieldNameLength, MinimumLength = BuildTableLayoutConsts.MincNormFieldNameLength)]
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
		
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcCreatedByLength, MinimumLength = BuildTableLayoutConsts.MincCreatedByLength)]
		public string cCreatedBy { get; set; }
		
		
		public DateTime? dModifiedDate { get; set; }
		
		
		[StringLength(BuildTableLayoutConsts.MaxcModifiedByLength, MinimumLength = BuildTableLayoutConsts.MincModifiedByLength)]
		public string cModifiedBy { get; set; }
		
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxLK_IndexTypeLength, MinimumLength = BuildTableLayoutConsts.MinLK_IndexTypeLength)]
		public string LK_IndexType { get; set; }
		
		
		public int? iKeyColumn { get; set; }
		
		
		public bool iIsRebuildDD { get; set; }
		
		
		[StringLength(BuildTableLayoutConsts.MaxcGroupNameLength, MinimumLength = BuildTableLayoutConsts.MincGroupNameLength)]
		public string cGroupName { get; set; }
		
		
		 public int BuildTableId { get; set; }
		 
		 
    }
}