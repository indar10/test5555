using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Occupations
{
    public interface IOccupationsRepository
    {
        List<DropdownOutputDto> GetOccupationValues(string query, List<SqlParameter> parameters);
    }
}