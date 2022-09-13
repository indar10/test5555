using Infogroup.IDMS.Divisions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Infogroup.IDMS.Databases
{
	[Table("tblDatabase")]
    [Audited]
    public class Database : Entity 
    {

		[Required]
		[StringLength(DatabaseConsts.MaxLK_DatabaseTypeLength, MinimumLength = DatabaseConsts.MinLK_DatabaseTypeLength)]
		public virtual string LK_DatabaseType { get; set; }
		
		[Required]
		[StringLength(DatabaseConsts.MaxcDatabaseNameLength, MinimumLength = DatabaseConsts.MincDatabaseNameLength)]
		public virtual string cDatabaseName { get; set; }
		
		[Required]
		[StringLength(DatabaseConsts.MaxcListFileUploadedPathLength, MinimumLength = DatabaseConsts.MincListFileUploadedPathLength)]
		public virtual string cListFileUploadedPath { get; set; }
		
		[Required]
		[StringLength(DatabaseConsts.MaxcListReadyToLoadPathLength, MinimumLength = DatabaseConsts.MincListReadyToLoadPathLength)]
		public virtual string cListReadyToLoadPath { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		[StringLength(DatabaseConsts.MaxcCreatedByLength, MinimumLength = DatabaseConsts.MincCreatedByLength)]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		[StringLength(DatabaseConsts.MaxcModifiedByLength, MinimumLength = DatabaseConsts.MincModifiedByLength)]
		public virtual string cModifiedBy { get; set; }
		
		[Required]
		[StringLength(DatabaseConsts.MaxLK_AccountingDivisionCodeLength, MinimumLength = DatabaseConsts.MinLK_AccountingDivisionCodeLength)]
		public virtual string LK_AccountingDivisionCode { get; set; }
		
		[StringLength(DatabaseConsts.MaxcAdministratorEmailLength, MinimumLength = DatabaseConsts.MincAdministratorEmailLength)]
		public virtual string cAdministratorEmail { get; set; }
		

		public virtual int DivisionId { get; set; }
		
        [ForeignKey("DivisionId")]
		public Division Division { get; set; }
		
    }
}