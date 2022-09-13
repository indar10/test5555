using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class MatchAppendAddFieldsDto
    {
        public List<string> SelectedFields { get; set; }
        public string SelectedTable { get; set; }

        public List<MatchAndAppendInputLayoutDto> MatchAppendInputLayoutList { get; set; }
        
        public  List<MatchAndAppendOutputLayoutDto> MatchAppendOutputLayoutList { get; set; }
    }
}
