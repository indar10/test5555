using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SubSelectLists.Dtos
{
    public class GetSubSelectSourcesForView : EntityDto<int?>
    {
        public int MasterLOLID { get; set; }

    }
}
