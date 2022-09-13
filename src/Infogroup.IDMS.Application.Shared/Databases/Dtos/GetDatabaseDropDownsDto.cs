using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Databases.Dtos
{
   public class GetDatabaseDropDownsDto
    {
       
        public List<DropdownOutputDto> Divisions;
        public int DefaultDivision { get; set; }
        public List<DropdownOutputDto> DatabaseTypes { get; set; }
        public string DefaultDatabaseType { get; set; }
        public List<DropdownOutputDto> DivisionCodes { get; set; }
        public string DefaultDivisionCode { get; set; }
    }
}
