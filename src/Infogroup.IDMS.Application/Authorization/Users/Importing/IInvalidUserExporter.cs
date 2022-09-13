using System.Collections.Generic;
using Infogroup.IDMS.Authorization.Users.Importing.Dto;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Authorization.Users.Importing
{
    public interface IInvalidUserExporter
    {
        FileDto ExportToFile(List<ImportUserDto> userListDtos);
    }
}
