using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.DivisionMailers.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.DivisionMailers
{
    public interface IDivisionMailersAppService : IApplicationService 
    {
        PagedResultDto<GetDivisionMailerForViewDto> GetAllDivisionMailerList(GetAllDivisionMailersInput filters);
        Task CreateOrEditDivisionMailer(CreateOrEditDivisionMailerDto input);
        Task<CreateOrEditDivisionMailerDto> GetDivisionMailerForEdit(int DivisionMailerId);
    }
}