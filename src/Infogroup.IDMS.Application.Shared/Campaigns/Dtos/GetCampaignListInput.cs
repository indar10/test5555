using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class GetListQuery : PagedAndSortedResultRequestDto
    {
        public string WhereClause { get; set; }
    }
}
