using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Editions.Dto;

namespace Infogroup.IDMS.MultiTenancy.Dto
{
    public class GetTenantFeaturesEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}