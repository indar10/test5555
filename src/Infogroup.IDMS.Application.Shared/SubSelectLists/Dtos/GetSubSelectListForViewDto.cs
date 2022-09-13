using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SubSelectLists.Dtos
{
    public class GetSubSelectListForViewDto : EntityDto<int?>
    {
        public string cListName { get; set; }
        public string LK_PermissionType { get; set; }
    }
}