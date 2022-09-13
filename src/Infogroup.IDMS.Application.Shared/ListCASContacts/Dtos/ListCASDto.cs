
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ListCASContacts.Dtos
{
   public class ListCASDto : EntityDto<int?>
    {

		public virtual int ContactID { get; set; }

		public virtual DateTime dCreatedDate { get; set; }

		public virtual string cCreatedBy { get; set; }

		public virtual DateTime? dModifiedDate { get; set; }

		public virtual string cModifiedBy { get; set; }

		public virtual int ListID { get; set; }

	}
}
