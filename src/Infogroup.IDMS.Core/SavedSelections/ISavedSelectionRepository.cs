using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SavedSelections.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SavedSelections
{
    public interface ISavedSelectionRepository : IRepository<SavedSelection, int>
    {
        Task<PagedResultDto<GetSavedSelectionForViewDto>> GetAllSavedSelection(int databaseID, int userID, GetAllSavedSelectionsInput Filter);

    }
}
