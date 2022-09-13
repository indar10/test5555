using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class GetSegmentListFilters: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public string ID { get; set; }
        public string Description { get; set; }
        
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
