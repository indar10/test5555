using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class GetAllTableDescriptionFromBuild
    {
        public List<DropdownOutputDto> ExportFlagFieldDropdown;
        public string DefaultSelection;
    }
}
