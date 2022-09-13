using Infogroup.IDMS.BuildTables;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.BuildTableLayouts
{
	[Table("tblBuildTableLayout")]
    public class BuildTableLayout : Entity 
    {

		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcFieldNameLength, MinimumLength = BuildTableLayoutConsts.MincFieldNameLength)]
		public virtual string cFieldName { get; set; }
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcFieldDescriptionLength, MinimumLength = BuildTableLayoutConsts.MincFieldDescriptionLength)]
		public virtual string cFieldDescription { get; set; }
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcDataTypeLength, MinimumLength = BuildTableLayoutConsts.MincDataTypeLength)]
		public virtual string cDataType { get; set; }
		
		public virtual int iDataLength { get; set; }
		
		public virtual bool iIsMailerSpecific { get; set; }
		
		public virtual bool iIsSystem { get; set; }
		
		public virtual bool iIsListSpecific { get; set; }
		
		public virtual bool iIsNormalized { get; set; }
		
		public virtual bool iIsEnhancement { get; set; }
		
		public virtual bool iIsMappingRequired { get; set; }
		
		public virtual bool iIsMappingAllowed { get; set; }
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcNormFieldNameLength, MinimumLength = BuildTableLayoutConsts.MincNormFieldNameLength)]
		public virtual string cNormFieldName { get; set; }
		
		public virtual bool iAuditType { get; set; }
		
		public virtual bool iIsSelectable { get; set; }
		
		public virtual bool iIsBillable { get; set; }
		
		public virtual bool iShowListBox { get; set; }
		
		public virtual bool iShowTextBox { get; set; }
		
		public virtual bool iFileOperations { get; set; }
		
		public virtual bool iShowDefault { get; set; }
		
		public virtual bool iAllowSorting { get; set; }
		
		public virtual bool iAllowMaxPer { get; set; }
		
		public virtual bool iLayoutOrder { get; set; }
		
		public virtual bool iAllowExport { get; set; }
		
		public virtual bool iDisplayOrder { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxcCreatedByLength, MinimumLength = BuildTableLayoutConsts.MincCreatedByLength)]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		[StringLength(BuildTableLayoutConsts.MaxcModifiedByLength, MinimumLength = BuildTableLayoutConsts.MincModifiedByLength)]
		public virtual string cModifiedBy { get; set; }
		
		[Required]
		[StringLength(BuildTableLayoutConsts.MaxLK_IndexTypeLength, MinimumLength = BuildTableLayoutConsts.MinLK_IndexTypeLength)]
		public virtual string LK_IndexType { get; set; }
		
		public virtual int? iKeyColumn { get; set; }
		
		public virtual bool iIsRebuildDD { get; set; }
		
		[StringLength(BuildTableLayoutConsts.MaxcGroupNameLength, MinimumLength = BuildTableLayoutConsts.MincGroupNameLength)]
		public virtual string cGroupName { get; set; }
		

		public virtual int BuildTableId { get; set; }
		
        [ForeignKey("BuildTableId")]
		public BuildTable BuildTable { get; set; }
		
    }
}