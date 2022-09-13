using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SICCodes
{
	[Table("tblSICCode")]
    public class SICCode : Entity
    {

		[Required]
		public virtual string cSICCode { get; set; }
		
		[Required]
		public virtual string cSICDescription { get; set; }
		
		public virtual string cType { get; set; }
		
		public virtual int? iPrimaryFlag { get; set; }
		
		public virtual string cIndicator { get; set; }
		

    }
}