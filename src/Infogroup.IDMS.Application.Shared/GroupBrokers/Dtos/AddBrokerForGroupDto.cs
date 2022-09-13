using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.GroupBrokers.Dtos
{
    public class AddBrokerForGroupDto
    {
        public int GroupID { get; set; }

        public List<GetGroupBrokerOutputDto> SelectedBroker { get; set; }
    }
}
