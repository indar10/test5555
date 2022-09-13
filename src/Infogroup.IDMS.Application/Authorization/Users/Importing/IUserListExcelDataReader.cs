using System.Collections.Generic;
using Infogroup.IDMS.Authorization.Users.Importing.Dto;
using Abp.Dependency;

namespace Infogroup.IDMS.Authorization.Users.Importing
{
    public interface IUserListExcelDataReader: ITransientDependency
    {
        List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes);
    }
}
