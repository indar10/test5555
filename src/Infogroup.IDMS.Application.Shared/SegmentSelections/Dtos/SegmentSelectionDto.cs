using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infogroup.IDMS.SegmentSelections.Dtos
{
    public class SegmentSelectionDto : EntityDto<int?>
    {
        public int SegmentId { get; set; }

        [StringLength(50)]
        public string cQuestionFieldName { get; set; }


        [StringLength(50)]
        public string cQuestionDescription { get; set; }

        [StringLength(10)]
        public string cJoinOperator { get; set; }

        public int iGroupNumber { get; set; }

        public int iGroupOrder { get; set; }

        public string cGrouping { get; set; }


        public string cValues { get; set; }

        [StringLength(1)]
        public string cValueMode { get; set; }


        public string cDescriptions { get; set; }

        [StringLength(50)]
        public string cValueOperator { get; set; }

        public string cFileName { get; set; }

        public string cSystemFileName { get; set; }


        public string cCreatedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime dCreatedDate { get; set; }


        public string cModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? dModifiedDate { get; set; }

        public string cTableName { get; set; }

        public int OrderID { get; set; }

        public int isRuleUpdated { get; set; }
        public int isDirectFileUpload { get; set; }

        public string cFieldDescription { get; set; }
    }


    public class RootSegmentSelectionDetails
    {
        [JsonProperty(Order = 1)]
        public string condition { get; set; }
        [JsonProperty(Order = 2)]
        public List<GroupSegmentSelectionDetails> rules = new List<GroupSegmentSelectionDetails>();


    }

    public class GroupSegmentSelectionDetails
    {
        [JsonProperty(Order = 1)]
        public string condition { get; set; }
        [JsonProperty(Order = 2)]
        public List<BindSegmentSelectionDetails> rules = new List<BindSegmentSelectionDetails>();
    }
    public class BindSegmentSelectionDetails
    {
        public string field { get; set; }
        public string input { get; set; }
        public int id { get; set; }
        public int selectionId { get; set; }
        public string Operator { get; set; }
        public string cJoinOperator { get; set; }
        public string value { get; set; }
        public int iGroupNumber { get; set; }
        public string cValueMode { get; set; }
        public string cFileName { get; set; }
        public string cSystemFileName { get; set; }
        public string cGrouping { get; set; }
        public bool iIsListSpecific { get; set; }
        public bool iIsRAWNotMapped { get; set; }
        public string cCreatedBy { get; set; }
        public DateTime? dCreatedDate { get; set; }
    }
    public class AdvanceSelectionsInputDto
    {
        public int SegmentID { get; set; }
        public List<SegmentSelectionDto> SICFields { get; set; }
        public SegmentSelectionDto PrimarySICField { get; set; }
    }
    public class ListSegmentFieldDetails
    {
        public List<string> listFieldName { get; set; }
        public List<string> listValues { get; set; }
        public List<string> listValueOperator { get; set; }
    }
    public class SegmentSelectionSaveDto
    {
        public List<SegmentSelectionDto> selections { get; set; }

        public int campaignId { get; set; }

        public List<int> deletedSelections { get; set; }

        public int DatabaseId { get; set; }

    }
}
