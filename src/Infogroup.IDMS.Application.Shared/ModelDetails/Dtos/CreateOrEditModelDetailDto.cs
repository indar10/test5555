
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ModelDetails.Dtos
{
    public class CreateOrEditModelDetailDto : EntityDto<int?>
    {
        public string cSQL_Score { get; set; }

        public string cSQL_Deciles { get; set; }

        public string cSQL_Preselect { get; set; }
    }
}