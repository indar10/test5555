using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infogroup.IDMS.Segments
{
    [Table("tblSegment")]
    public class Segment : Entity
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string cDescription { get; set; }

        public bool iUseAutosuppress { get; set; }

        [Required]
        [StringLength(50)]
        public string cKeyCode1 { get; set; }

        [Required]
        [StringLength(15)]
        public string cKeyCode2 { get; set; }

        [Required]
        public int iRequiredQty { get; set; }

        [Required]
        public int iProvidedQty { get; set; }


        [Required]
        public int iDedupeOrderSpecified { get; set; }

        [Required]
        public int iDedupeOrderUsed { get; set; }

        [StringLength(2)]
        public string cMaxPerGroup { get; set; }

        [Required]
        [StringLength(1)]
        public string cTitleSuppression { get; set; }

        [Required]
        [StringLength(50)]
        public string cFixedTitle { get; set; }

        [Required]
        public int iDoubleMultiBuyerCount { get; set; }


        [Required]
        public bool iIsOrderLevel { get; set; }

        [Required]
        [Column(TypeName = "smalldatetime")]
        public DateTime? dCreatedDate { get; set; }


        [Required]
        [StringLength(25)]
        public string cCreatedBy { get; set; }


        [Column(TypeName = "smalldatetime")]
        public DateTime? dModifiedDate { get; set; }


        [StringLength(25)]
        public string cModifiedBy { get; set; }


        public int? iGroup { get; set; }

        public int? iOutputQty { get; set; }

        public int? iAvailableQty { get; set; }

        public bool iIsRandomRadiusNth { get; set; }

        public string cNthPriorityField { get; set; } = string.Empty;

        [StringLength(1)]
        public string cNthPriorityFieldOrder { get; set; } = string.Empty;
    }
}