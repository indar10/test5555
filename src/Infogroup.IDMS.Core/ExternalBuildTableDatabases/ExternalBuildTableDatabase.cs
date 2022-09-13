using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ExternalBuildTableDatabases
{
	[Table("tblExternalBuildTableDatabase")]
    public class ExternalBuildTableDatabase : Entity 
    {

		public virtual int? DatabaseID { get; set; }
		
		public virtual int? BuildTableID { get; set; }
		
		public virtual DateTime? dCreatedDate { get; set; }
		
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}