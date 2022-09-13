using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ListAutomate.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.ListAutomate
{
    public interface IListAutomatesAppService : IApplicationService
    {
        Task<CreateOrEditIListAutomateDto> GetIListAutomateForEdit(EntityDto id);

        Task CreateOrEdit(CreateOrEditIListAutomateDto input);

        List<DropdownOutputDto> GetFrequency();

        GetServerDateTime GetServerDateForTime();       

    }
}