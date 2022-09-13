using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
namespace Infogroup.IDMS.Builds.Dtos
{
    public class GetAllBuildDropdownOutputDto
    {
        public List<DropdownOutputDto> BuildDropDown;
        public int DefaultSelection;
    }
}
