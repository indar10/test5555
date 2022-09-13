
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SavedSelectionDetails.Dtos
{
    public class SavedSelectionDetailDto : EntityDto
    {

        public string cQuestionFieldName { get; set; }

        public string cQuestionDescription { get; set; }

        public string cJoinOperator { get; set; }

        public int iGroupNumber { get; set; }

        public int iGroupOrder { get; set; }
       
        public string cGrouping { get; set; }
       
        public string cValues { get; set; }
       
        public string cValueMode { get; set; }
       
        public string cDescriptions { get; set; }

        public string cValueOperator { get; set; }

        public bool iIsActive { get; set; }

        public DateTime dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

        public string cTableName { get; set; }

        public int SavedSelectionId { get; set; }

    }
}