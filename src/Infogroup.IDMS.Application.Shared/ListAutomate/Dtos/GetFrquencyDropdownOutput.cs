using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ListAutomate.Dtos
{
    public class GetFrquencyDropdownOutput
    {
        public List<DropdownOutputDto> FrequencyDropDown;
        public int DefaultSelection { get; set; }
    }
}
