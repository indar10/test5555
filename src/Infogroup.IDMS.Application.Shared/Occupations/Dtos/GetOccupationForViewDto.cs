using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Occupations.Dtos
{
    public class GetOccupationForViewDto
    {
        public AdvanceSelectionFields ConfiguredFields { get; set; }
        public List<DropdownOutputDto> Industries { get; set; }
    }
  
}