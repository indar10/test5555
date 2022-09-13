using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Mailers.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Mailers
{
    public interface IMailersAppService : IApplicationService
    {
        PagedResultDto<MailerDto> GetAllMailers(GetAllSetupInput input);

        Task<CreateOrEditMailerDto> GetMailerForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditMailerDto input);

        FileDto ExportAllMailerToExcel(GetAllSetupInput input);

        FileDto ExportOffers(GetAllSetupInput input);

        List<DropdownOutputDto> GetAllBrokersbyDatabaseId(int databaseId);

    }
}