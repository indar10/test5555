using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Redis
{
    public class RedisCacheDto : EntityDto
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public string Count { get; set; }
    }
}
