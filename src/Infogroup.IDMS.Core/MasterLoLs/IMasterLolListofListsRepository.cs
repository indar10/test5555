using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ListCASContacts.Dtos;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.MasterLoLs.Dtos;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.MasterLoLs
{
    public interface IMasterLolListofListsRepository : IRepository<MasterLoL, int>
    {
        PagedResultDto<MasterLoLForViewDto> GetAllListsOfList(Tuple<string, string, List<SqlParameter>> query);

        List<DropdownOutputDto> GetAllOwnersforlistoflist(string Query, List<SqlParameter> sqlParameters);

        List<DropdownOutputDto> GetAllManagersforlistoflist(string Query, List<SqlParameter> sqlParameters);

        List<DropdownOutputDto> GetAllMailersforlistoflist(string Query, List<SqlParameter> sqlParameters);
        List<DropdownOutputDto> GetAllAvailableMailersforlistoflist(string Query, List<SqlParameter> sqlParameters);

        PagedResultDto<LookupForListofListDto> GetallDropdownsfromLookups(Tuple<string, string, List<SqlParameter>> query);

        PagedResultDto<ContactTableDto> GetallContacts(Tuple<string, string, List<SqlParameter>> query);
        
        CreateOrEditMasterLoLDto GetListsofListById(Tuple<string, string, List<SqlParameter>> query);

        PagedResultDto<CreateOrEditListMailerDto> GetAvailableMailersById(Tuple<string, string, List<SqlParameter>> query);

        PagedResultDto<CreateOrEditListCASContacts> GetCASContactsById(Tuple<string, string, List<SqlParameter>> query);

        PagedResultDto<CreateOrEditListMailerRequestedDto> GetReqMailersById(Tuple<string, string, List<SqlParameter>> query);

        PagedResultDto<CreateOrEditMasterLoLDto> GetAllListsName(Tuple<string, string, List<SqlParameter>> query);
        List<ExportListMailerAccess> GetallDataForExportToListMaierAccess(string query);
        IEnumerable<ExportToExcelMasterLolDto> GetallDataForExportToMaierAccess(string query);
      
    }
}
