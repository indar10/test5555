using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Decoys.Dtos
{
    public class GetDecoyForViewDto
    {
        public DecoyDto Decoy { get; set; }

        public List<DecoyDto> listOfDecoys { get; set; }

        public int isDecoyKeyMethod { get; set; }
        public bool decoyByKeyCode { get; set; }
        public string decoyKey { get; set; }
        public string decoyKey1 { get; set; }
        public List<DropdownOutputDto> listOfDecoyGroup { get; set; }
        public string DatabasecDatabaseName { get; set;}

        public List<DropdownOutputDto> listOfGroupsForEdit { get; set; }
    }
}