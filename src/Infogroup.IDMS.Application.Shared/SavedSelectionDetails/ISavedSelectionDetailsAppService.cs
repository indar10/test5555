using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SavedSelectionDetails.Dtos;
using Infogroup.IDMS.Dto;
using System.Collections.Generic;

namespace Infogroup.IDMS.SavedSelectionDetails
{
    public interface ISavedSelectionDetailsAppService : IApplicationService 
    {
        Task<List<GetSavedSelectionDetailForViewDto>> GetAllSavedSelectionsDetails(GetAllSavedSelectionDetailsInput input);
    }
}