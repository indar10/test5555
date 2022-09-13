using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.GroupBrokers.Dtos
{
    public class GetAllBrokersInputDto       
    {
        public string Filter { get; set; }

        public int GroupId { get; set; }

        public int DatabaseId { get; set; }

        public string Sorting { get; set; }
    }
    
}
