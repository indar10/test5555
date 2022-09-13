
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.BuildTables.Dtos
{
    public class CreateOrEditBuildTableDto : EntityDto<int?>
    {

		[Required]
		[StringLength(BuildTableConsts.MaxcTableNameLength, MinimumLength = BuildTableConsts.MincTableNameLength)]
		public string cTableName { get; set; }
		
		
		[Required]
		[StringLength(BuildTableConsts.MaxLK_TableTypeLength, MinimumLength = BuildTableConsts.MinLK_TableTypeLength)]
		public string LK_TableType { get; set; }
		
		
		[Required]
		[StringLength(BuildTableConsts.MaxLK_JoinTypeLength, MinimumLength = BuildTableConsts.MinLK_JoinTypeLength)]
		public string LK_JoinType { get; set; }
		
		
		[Required]
		[StringLength(BuildTableConsts.MaxcJoinOnLength, MinimumLength = BuildTableConsts.MincJoinOnLength)]
		public string cJoinOn { get; set; }
		
		
		public DateTime dCreatedDate { get; set; }
		
		
		[Required]
		[StringLength(BuildTableConsts.MaxcCreatedByLength, MinimumLength = BuildTableConsts.MincCreatedByLength)]
		public string cCreatedBy { get; set; }
		
		
		public string dModifiedDate { get; set; }
		
		
		[StringLength(BuildTableConsts.MaxcModifiedByLength, MinimumLength = BuildTableConsts.MincModifiedByLength)]
		public string cModifiedBy { get; set; }
		
		
		[Required]
		[StringLength(BuildTableConsts.MaxctabledescriptionLength, MinimumLength = BuildTableConsts.MinctabledescriptionLength)]
		public string ctabledescription { get; set; }
		
		
		 public int BuildId { get; set; }
		 
		 
    }
}