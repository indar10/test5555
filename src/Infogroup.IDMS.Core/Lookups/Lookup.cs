using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Lookups
{
    [Table("tblLookup")]
    public class Lookup : Entity
    {
        [Required]
        [StringLength(LookupConsts.MaxcLookupValueLength, MinimumLength = LookupConsts.MincLookupValueLength)]
        public virtual string cLookupValue { get; set; }

        public virtual int iOrderBy { get; set; }

        [Required]
        [StringLength(LookupConsts.MaxcCodeLength, MinimumLength = LookupConsts.MincCodeLength)]
        public virtual string cCode { get; set; }

        [Required]
        [StringLength(LookupConsts.MaxcDescriptionLength, MinimumLength = LookupConsts.MincDescriptionLength)]
        public virtual string cDescription { get; set; }

        [Required]
        [StringLength(LookupConsts.MaxcFieldLength, MinimumLength = LookupConsts.MincFieldLength)]
        public virtual string cField { get; set; }

        public virtual string mField { get; set; }

        public virtual int iField { get; set; }

        public virtual bool iIsActive { get; set; }

     
        [StringLength(LookupConsts.MaxcCreatedByLength, MinimumLength = LookupConsts.MincCreatedByLength)]
        public virtual string cCreatedBy { get; set; }

        [StringLength(LookupConsts.MaxcModifiedByLength, MinimumLength = LookupConsts.MincModifiedByLength)]
        public virtual string cModifiedBy { get; set; }

        public virtual DateTime dCreatedDate { get; set; }

        public virtual DateTime? dModifiedDate { get; set; }


    }
}