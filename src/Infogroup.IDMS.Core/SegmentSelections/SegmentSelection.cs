using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infogroup.IDMS.SegmentSelections
{
    [Table("tblSegmentSelection")]
    public class SegmentSelection: Entity
    {
        [Required]
        public int SegmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string cQuestionFieldName { get; set; }

        
        [StringLength(50)]
        public string cQuestionDescription { get; set; }

        [Required]
        [StringLength(10)]
        public string cJoinOperator { get; set; }

        [Required]
        public int iGroupNumber { get; set; }

        [Required]
        public int iGroupOrder { get; set; }

        [Required]
        [StringLength(1)]
        public string cGrouping { get; set; }

       
        public string cValues { get; set; }

        [Required]
        [StringLength(1)]
        public string cValueMode { get; set; }

        
        public string cDescriptions { get; set; }

        [Required]
        [StringLength(50)]
        public string cValueOperator { get; set; }

        public string cFileName { get; set; }

        public string cSystemFileName { get; set; }

        [Required]
        [StringLength(25)]
        public string cCreatedBy { get; set; }

        [Required]
        [Column(TypeName = "smalldatetime")]
        public DateTime dCreatedDate { get; set; }
 
        [StringLength(25)]
        public string cModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? dModifiedDate { get; set; }

        public string cTableName { get; set; }

        

    }
}
