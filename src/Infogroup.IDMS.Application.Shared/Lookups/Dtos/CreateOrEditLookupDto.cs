
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Lookups.Dtos
{
    public class CreateOrEditLookupDto : EntityDto<int?>
    {

        [StringLength(LookupConsts.MaxcLookupValueLength, MinimumLength = LookupConsts.MincLookupValueLength)]
        public string cLookupValue { get; set; }


        public int iOrderBy { get; set; }


        [StringLength(LookupConsts.MaxcCodeLength, MinimumLength = LookupConsts.MincCodeLength)]
        public string cCode { get; set; }

        [StringLength(LookupConsts.MaxcDescriptionLength, MinimumLength = LookupConsts.MincDescriptionLength)]
        public string cDescription { get; set; }


        [StringLength(LookupConsts.MaxcFieldLength, MinimumLength = LookupConsts.MincFieldLength)]
        public string cField { get; set; }


        public string mField { get; set; }


        public int iField { get; set; }


        public bool iIsActive { get; set; }


        [StringLength(LookupConsts.MaxcCreatedByLength, MinimumLength = LookupConsts.MincCreatedByLength)]
        public string cCreatedBy { get; set; }


        [StringLength(LookupConsts.MaxcModifiedByLength, MinimumLength = LookupConsts.MincModifiedByLength)]
        public string cModifiedBy { get; set; }


        public DateTime dCreatedDate { get; set; }


        public DateTime? dModifiedDate { get; set; }


    }
}