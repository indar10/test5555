using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.States.Dtos
{
    public class GetStateForViewDto
    {
        public AdvanceSelectionFields ConfiguredFields { get; set; }
        public int TargetDatabaseId { get; set; }
        public List<DropdownOutputDto> States { get; set; }
    }
}