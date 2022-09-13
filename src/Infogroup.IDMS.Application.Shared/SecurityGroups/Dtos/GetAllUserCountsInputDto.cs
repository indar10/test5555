using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SecurityGroups.Dtos

{
    public class GetAllUserCountsInputDto
    {
        public string Filter { get; set; }

        public int GroupID { get; set; }

        public string Sorting { get; set; }
    }
}
