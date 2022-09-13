using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Mailers.Dtos
{
    public class GetAllBrokerDropdownOutputDto
    {
        public List<DropdownOutputDto> BrokerDropDown;

        public int DefaultSelection { get; set; }
    }
}