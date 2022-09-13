using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Models.Dtos
{
    public class GetAllDatabaseDto
    {
        public List<DropdownOutputDto> Databases { get; set; }

        public int DefaultDatabase { get; set; }
    }
}
