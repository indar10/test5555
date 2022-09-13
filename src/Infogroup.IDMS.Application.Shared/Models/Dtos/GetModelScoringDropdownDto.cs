using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Models.Dtos
{
    public class GetModelScoringDropdownDto
    {
        public GetAllDatabaseDto Databases { get; set; }
        public GetAllBuildsDto Builds { get; set; }
    }
}
