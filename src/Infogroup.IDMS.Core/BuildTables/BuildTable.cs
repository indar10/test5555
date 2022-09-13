using Infogroup.IDMS.Builds;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.BuildTables
{
	[Table("tblBuildTable")]
    public class BuildTable : Entity 
    {

		[Required]
		public virtual string cTableName { get; set; }
		
		[Required]
		public virtual string LK_TableType { get; set; }
		
		[Required]
		public virtual string LK_JoinType { get; set; }
		
		[Required]
		public virtual string cJoinOn { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		[Required]
		public virtual string ctabledescription { get; set; }
		

		public virtual int BuildId { get; set; }
		
        [ForeignKey("BuildId")]
		public Build Build { get; set; }
		
    }
}