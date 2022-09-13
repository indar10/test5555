using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SavedSelectionDetails.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SavedSelectionDetails
{
    public interface ISavedSelectionDetailRepository : IRepository<SavedSelectionDetail, int>
    {
        Task<List<GetSavedSelectionDetailForViewDto>> GetAllSavedSelectionDetail(GetAllSavedSelectionDetailsInput input);
    }
}
