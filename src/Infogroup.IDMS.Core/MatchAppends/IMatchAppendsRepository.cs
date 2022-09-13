using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.MatchAppends.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.MatchAppends
{
    public interface IMatchAppendsRepository : IRepository<MatchAppend, int>
    {
        PagedResultDto<GetMatchAppendForViewDto> GetAllMatchAppendTasksList(Tuple<string, string, List<SqlParameter>> query);
        void CopyMatchAppendTask(int matchAppendId, string userId);
        List<DropdownOutputDto> GetAllDatabasesFromUserId(Tuple<string, List<SqlParameter>> query);
        
        List<DropdownOutputDto> GetMatchAppendAvailableFields(int tableId);
        List<MatchAndAppendStatusDto> GetMatchAppendStatusDetails(int matchAppendID);
    }
}
