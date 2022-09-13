using Abp.Application.Services.Dto;
using Infogroup.IDMS.SubSelects.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SubSelectSelections.Dtos
{
	public class SubSelectSelectionsDTO : EntityDto
	{
		public string cTableName { get; set; }
		public int SubSelectId { get; set; }

		public string cJoinOperator { get; set; }

		public string cQuestionFieldName { get; set; }

		public string cQuestionDescription { get; set; }

		public int iGroupNumber { get; set; }

		public int iGroupOrder { get; set; }

		public string cGrouping { get; set; }

		public string cValues { get; set; }

		public string cDescriptions { get; set; }

		public string cValueMode { get; set; }

		public string cValueOperator { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public DateTime? dModifiedDate { get; set; }

		public string cModifiedBy { get; set; }

		public bool iIsListSpecific { get; set; }
		public bool iIsRAWNotMapped { get; set; }
		public int addedFilterId { get; set; }
		public int CampaignId { get; set; }
	}
	public class SubSelectSelectionsDetailsDto
	{
		public List<SubSelectSelectionsDTO> Fields { get; set; }
		public int BuildLolId { get; set; }
	}
}
