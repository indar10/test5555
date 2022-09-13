using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class MatchAndAppendStatusDto
    {
        public int Id { get; set; }
        public string StatusDescription { get; set; }
        
        public string dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }

    }
}
