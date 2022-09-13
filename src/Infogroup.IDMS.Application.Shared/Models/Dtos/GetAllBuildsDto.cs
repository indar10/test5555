using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Models.Dtos
{
    public class GetAllBuildsDto
    {
        public List<DropdownOutputDto> BuildDropDown;
        public int DefaultSelection;
    }
}
