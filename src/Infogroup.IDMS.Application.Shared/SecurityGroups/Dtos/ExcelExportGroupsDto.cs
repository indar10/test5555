using Abp.Application.Services.Dto;
using Infogroup.IDMS.IDMSUsers.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SecurityGroups.Dtos
{
    public class ExcelExportGroupsDto: EntityDto
    {
        public int DatabaseID { get; set; }

        public string cGroupName { get; set; }

        public int UserCount { get; set; }

        public string cGroupDescription { get; set; }

        public string cStatus { get; set; }

        public bool iIsActive { get; set; }

        public List<string> GroupBrokerList { get; set; }

        public List<string> GroupUsers { get; set; }
    }
}
