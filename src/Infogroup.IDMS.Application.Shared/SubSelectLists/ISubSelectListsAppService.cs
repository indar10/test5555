using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SubSelectLists.Dtos;
using Infogroup.IDMS.Dto;
using System.Collections.Generic;

namespace Infogroup.IDMS.SubSelectLists
{
    public interface ISubSelectListsAppService : IApplicationService 
    {
        Task<List<GetSubSelectSourcesForView>> GetSubSelectSourcesForEdit(int subSelectId);

    }
}