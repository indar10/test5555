using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Shared
{
    public interface ISharedRepository
    {
        List<DropdownOutputDto> GetDropdownOptionsForAlphaNumericValues(Tuple<string, List<SqlParameter>> query);
        List<DropdownOutputDto> GetDropdownOptionsForNumericValues(Tuple<string, List<SqlParameter>> query);
    }
}