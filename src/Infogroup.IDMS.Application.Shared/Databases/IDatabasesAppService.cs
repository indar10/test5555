using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Databases.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Databases
{
    public interface IDatabasesAppService : IApplicationService 
    {
        Task<PagedResultDto<DatabaseDto>> GetAllDatabases(GetAllDatabasesInput input);

		Task<CreateOrEditDatabaseDto> GetDatabaseForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditDatabaseDto input);
        GetDatabaseDropDownsDto GetDatabaseDropDownsDto(int userId, int databaseId);

        FileDto ExportDatabaseExcel(int databaseId, string databaseName);
        
    }
}