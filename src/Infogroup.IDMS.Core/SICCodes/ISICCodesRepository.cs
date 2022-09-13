using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.SICCodes.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.SICCodes
{
    public interface ISICCodesRepository
    {
        int GetSICCodesCount(Tuple<string, List<SqlParameter>> query);
        List<SICCodeDto> GetSICCodes(Tuple<string, List<SqlParameter>> query);
        List<DropdownOutputDto> GetFranchiseIndustryBySIC(Tuple<string, List<SqlParameter>> query);
        (List<SICCode>, List<string>) GetSICCodesForSmartAdd(Tuple<string, List<SqlParameter>> query);        
    }
}