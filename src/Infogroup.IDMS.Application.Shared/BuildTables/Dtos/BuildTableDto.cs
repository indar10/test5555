
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.BuildTables.Dtos
{
    public class BuildTableDto : EntityDto
    {
		public string cTableName { get; set; }

		public string LK_TableType { get; set; }

		public string LK_JoinType { get; set; }

		public string cJoinOn { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public string dModifiedDate { get; set; }

		public string cModifiedBy { get; set; }

		public string ctabledescription { get; set; }


		 public int BuildId { get; set; }

		 
    }
}