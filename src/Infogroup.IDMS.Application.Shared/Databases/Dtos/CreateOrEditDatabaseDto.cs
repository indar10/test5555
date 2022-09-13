
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Databases.Dtos
{
    public class CreateOrEditDatabaseDto : EntityDto<int?>
    {

		[Required]
		[StringLength(DatabaseConsts.MaxLK_DatabaseTypeLength, MinimumLength = DatabaseConsts.MinLK_DatabaseTypeLength)]
		public string LK_DatabaseType { get; set; }
		
		
		[Required]
		[StringLength(DatabaseConsts.MaxcDatabaseNameLength, MinimumLength = DatabaseConsts.MincDatabaseNameLength)]
		public string cDatabaseName { get; set; }
		
		
		[Required]
		[StringLength(DatabaseConsts.MaxcListFileUploadedPathLength, MinimumLength = DatabaseConsts.MincListFileUploadedPathLength)]
		public string cListFileUploadedPath { get; set; }
		
		
		[Required]
		[StringLength(DatabaseConsts.MaxcListReadyToLoadPathLength, MinimumLength = DatabaseConsts.MincListReadyToLoadPathLength)]
		public string cListReadyToLoadPath { get; set; }
		
		
		public DateTime dCreatedDate { get; set; }
		
		
		
		[StringLength(DatabaseConsts.MaxcCreatedByLength, MinimumLength = DatabaseConsts.MincCreatedByLength)]
		public string cCreatedBy { get; set; }
		
		
		public DateTime? dModifiedDate { get; set; }
		
		
		[StringLength(DatabaseConsts.MaxcModifiedByLength, MinimumLength = DatabaseConsts.MincModifiedByLength)]
		public string cModifiedBy { get; set; }
		
		
		[Required]
		[StringLength(DatabaseConsts.MaxLK_AccountingDivisionCodeLength, MinimumLength = DatabaseConsts.MinLK_AccountingDivisionCodeLength)]
		public string LK_AccountingDivisionCode { get; set; }
		
		
		[StringLength(DatabaseConsts.MaxcAdministratorEmailLength, MinimumLength = DatabaseConsts.MincAdministratorEmailLength)]
		public string cAdministratorEmail { get; set; }

        public int DivisionId { get; set; }

    }
}