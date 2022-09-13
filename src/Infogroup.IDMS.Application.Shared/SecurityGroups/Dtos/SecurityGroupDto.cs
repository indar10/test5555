
using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SecurityGroups.Dtos
{
    public class SecurityGroupDto : EntityDto
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