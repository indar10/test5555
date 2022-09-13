using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSConfigurations.Dtos
{
	public class CreateOrEditConfigurationDto: EntityDto<int?>
    {
		public  int DivisionID { get; set; }

		public  int DatabaseID { get; set; }

		public  string cItem { get; set; }

		public  string cDescription { get; set; }

		public  string cValue { get; set; }

		public  int iValue { get; set; }

		public  DateTime? dValue { get; set; }

		public  string mValue { get; set; }

		public  bool iIsActive { get; set; }

		public  string cCreatedBy { get; set; }

		public  string cModifiedBy { get; set; }

		public  DateTime dCreatedDate { get; set; }

		//public string dCreatedDate { get; set; }

		public  DateTime? dModifiedDate { get; set; }

		public  bool iIsEncrypted { get; set; }


	}
}
