using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SubSelectSelections.Dtos;
using Infogroup.IDMS.Dto;
using System.Collections.Generic;

namespace Infogroup.IDMS.SubSelectSelections
{
    public interface ISubSelectSelectionsAppService : IApplicationService 
    {
		SubSelectSelectionsDetailsDto GetAllSubSelectSelections(GetAllSubSelectSelectionsInput input);

		Task CreateOrEditSubSelectSelection(SubSelectSelectionsDTO input);

		Task DeleteSubSelectSelection(int subSelectSelectionId, int addedFilterId, int campaignId);

		
    }
}