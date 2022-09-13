using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.ListCASContacts.Dtos
{
  public  class CreateOrEditListCASContacts : EntityDto<int?>
    {
		public  int ContactID { get; set; }

		public  DateTime dCreatedDate { get; set; }

		public  string cCreatedBy { get; set; }

		public  DateTime? dModifiedDate { get; set; }

		public  string cModifiedBy { get; set; }

		public  int ListID { get; set; }

		public ActionType Action { get; set; }
	}
}
