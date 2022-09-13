using Abp.Domain.Repositories;
using Infogroup.IDMS.Databases.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Databases
{
    public interface IDatabaseRepository: IRepository<Database, int>
    {
        DatabaseDto GetDataSetDatabaseByOrderID(int iOrderID);
        DatabaseDto GetDatabaseIDByOrderID(int iOrderID);
        List<DropdownOutputDto> FetchDatabasesWithAccess(int iUserID);
        List<DropdownOutputDto> GetDatabaseData(string Query, List<SqlParameter> sqlParameters);
    }
}
