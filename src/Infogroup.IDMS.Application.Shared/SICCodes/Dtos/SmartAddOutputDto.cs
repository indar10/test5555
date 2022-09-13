using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.SICCodes.Dtos
{
    public class SmartAddOutputDto
    {
        public string WarningMessage { get; set; }
        public List<List<DropdownOutputDto>> SICSelections { get; set; }
    }
}