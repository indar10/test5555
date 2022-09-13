using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.GroupBrokers.Dtos
{
    public class GetGroupBrokerOutputDto
    {
        public int Id { get; set; }

        public string cCode { get; set; }

        public string cCompany { get; set; }

        public bool IsSelected { get; set; }
    }
}