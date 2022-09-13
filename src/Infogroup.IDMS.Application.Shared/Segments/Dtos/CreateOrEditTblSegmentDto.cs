
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class CreateOrEditSegmentDto : EntityDto<int?>
    {
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string cDescription { get; set; }

        public bool iUseAutosuppress { get; set; }

        [StringLength(50)]
        public string cKeyCode1 { get; set; }

        [StringLength(15)]
        public string cKeyCode2 { get; set; }

        public int iRequiredQty { get; set; }

        public int iProvidedQty { get; set; }

        public int iDedupeOrderSpecified { get; set; }

        public int iDedupeOrderUsed { get; set; }

        [StringLength(2)]
        public string cMaxPerGroup { get; set; }

        [StringLength(1)]
        public string cTitleSuppression { get; set; }

        [StringLength(50)]
        public string cFixedTitle { get; set; }

        public int iDoubleMultiBuyerCount { get; set; }

        public bool iIsOrderLevel { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? dCreatedDate { get; set; }

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

        public string cFieldDescription { get; set; }

        public bool ApplyDefaultRules { get; set; }

        public bool iIsCalculateDistance { get; set; }

        public string cNthPriorityField { get; set; }

        public string cNthPriorityFieldDescription { get; set; }

        [StringLength(1)]
        public string cNthPriorityFieldOrder { get; set; }
    }
}