using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Infogroup.IDMS.IDMSConfigurations.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;
using System.Threading.Tasks;

namespace Infogroup.IDMS.IDMSConfigurations
{
    public interface IIDMSConfigurationsAppService : IApplicationService
    {        
        IDMSConfigurationDto GetConfigurationValue(string cItem, int databaseId);
        PagedResultDto<GetAllConfigurationsForViewDto> GetAllConfiguration(GetAllConfigurationsListInput input);
        List<DropdownOutputDto> GetConfigurationItems();
        Task CreateOrEditIDMSConfig(CreateOrEditConfigurationDto input);
        Task<CreateOrEditConfigurationDto> GetConfigurationForEdit(EntityDto input);
        IEnumerable<IDMSConfigurationDto> GetMultipleConfigurationsValue(List<string> cItem, int databaseId);
    }
}
