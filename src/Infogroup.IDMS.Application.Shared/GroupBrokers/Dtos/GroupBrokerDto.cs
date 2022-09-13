
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.GroupBrokers.Dtos
{
    public class GroupBrokerDto : EntityDto
    {
        public string cCode { get; set; }

        public string cCompany { get; set; }

        public bool isSelected { get; set; }
    }
}