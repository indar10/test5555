using Abp.Domain.Repositories;
using Infogroup.IDMS.SubSelectSelections.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SubSelectSelections
{
    public interface ISubSelectSelectionsRepository : IRepository<SubSelectSelection, int>
    {
        List<SubSelectSelectionsDTO> GetAllSubSelectSelections(int iSubSelectID, int iBuildLoLID);
        void UpdateSubSelectSelection(int selectionId, string cGrouping);


    }
}
