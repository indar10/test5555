using System.Collections.Generic;
using Infogroup.IDMS.Authorization.Users.Dto;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}