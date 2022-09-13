using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.GroupBrokers.Dtos
{
    public class GetAllGroupBrokersInputDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int GroupId { get; set; }

        public int DatabaseId { get; set; }
    }
}