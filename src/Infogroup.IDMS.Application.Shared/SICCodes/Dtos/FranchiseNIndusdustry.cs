using System.Collections.Generic;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.SICCodes.Dtos
{

    public class FranchiseNIndusdustry
    {
        public List<DropdownOutputDto> Franchises { get; set; } = new List<DropdownOutputDto>();
        public List<DropdownOutputDto> Industries { get; set; } = new List<DropdownOutputDto>();
    }
}