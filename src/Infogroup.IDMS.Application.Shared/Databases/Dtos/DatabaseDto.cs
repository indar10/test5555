using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Databases.Dtos
{
    public class DatabaseDto : EntityDto
    {
		public string LK_DatabaseType { get; set; }

		public string cDatabaseName { get; set; }

		public string cListFileUploadedPath { get; set; }

		public string cListReadyToLoadPath { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public DateTime? dModifiedDate { get; set; }

		public string cModifiedBy { get; set; }

		public string LK_AccountingDivisionCode { get; set; }

		public string cAdministratorEmail { get; set; }

        public string DivisonName { get; set; }

        public int? DivisionId { get; set; }
		 
    }
}