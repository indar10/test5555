using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SICFranchiseCodes
{
	[Table("tblSICFranchiseCode")]
    public class SICFranchiseCode : Entity 
    {

		[Required]
		public virtual string cSICCode { get; set; }
		
		[Required]
		public virtual string cSICDescription { get; set; }
		
		[Required]
		public virtual string cFranchiseCode { get; set; }
		
		[Required]
		public virtual string cConvertedFranchise { get; set; }
		
		[Required]
		public virtual string cFranchiseName { get; set; }
		
		[Required]
		public virtual string cFranchiseType { get; set; }
		
		[Required]
		public virtual string cCanadianFlag { get; set; }
		
		[Required]
		public virtual string cOldFranchiseCode { get; set; }
		
		[Required]
		public virtual string cTransactionDate { get; set; }
		
		[Required]
		public virtual string CRLF { get; set; }
		

    }
}