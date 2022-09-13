using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SavedSelections.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.SavedSelections
{
    public interface ISavedSelectionsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSavedSelectionForViewDto>> GetAllSavedSelections(GetAllSavedSelectionsInput input);
        void DeleteSavedSelectionAsync(int id);
        void DeleteUserSavedSelectionAsync(int id);
    }
}