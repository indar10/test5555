using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SubSelects.Dtos
{
    public class SubSelectForViewDto : EntityDto<int>
    {
        public string cIncludeExclude { get; set; }
        public string cCompanyIndividual { get; set; }
    }
}