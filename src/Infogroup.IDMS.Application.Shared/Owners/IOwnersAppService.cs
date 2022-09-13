using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Owners.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Owners
{
    public interface IOwnersAppService : IApplicationService 
    {

        Task<CreateOrEditOwnerDto> GetOwnerForEdit(EntityDto input);

        PagedResultDto<OwnerDto> GetAllOwners(GetAllSetupInput input);

        Task CreateOrEdit(CreateOrEditOwnerDto input);

        FileDto ExportToExcel(GetAllSetupInput input);

    }
}