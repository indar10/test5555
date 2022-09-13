using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ExternalBuildTableDatabases.Dtos;
using Infogroup.IDMS.Dto;
using System.Collections.Generic;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.ExternalBuildTableDatabases
{
    public interface IExternalBuildTableDatabasesAppService : IApplicationService 
    {
        PagedResultDto<ExternalBuildTableDatabaseForAllDto> GetAllLinks(GetAllForLookupTableInput input,bool exporttoexcel);
        List<DropdownOutputDto> GetAllBuildTableDescForDropdown();
        Task deleteRecord(int id);
        Task CreateOrEdit(CreateOrEditExternalBuildTableDatabaseDto input);
        FileDto ExportAllExternalDbLinksToExcel(GetAllForLookupTableInput input, bool exporttoexcel);


    }
}