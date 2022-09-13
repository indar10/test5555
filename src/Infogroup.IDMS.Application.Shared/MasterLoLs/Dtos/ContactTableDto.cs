using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.MasterLoLs.Dtos
{
    public class ContactTableDto: PagedAndSortedResultRequestDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string email { get; set; }
    }
}
