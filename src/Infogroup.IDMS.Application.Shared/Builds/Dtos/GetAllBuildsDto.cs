using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Builds.Dtos
{
    public class GetAllBuildsForDatabaseDto
    {
        public List<DropdownOutputDto> BuildDropDown;
        public int DefaultSelection;
    }
}
