using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.MasterLoLs.Dtos
{
   public class GetOwnerForListofListDto : PagedAndSortedResultRequestDto
    {
        public int Id { get; set; }

        public string cCode { get; set; }

        public string cCompany { get; set; }

        public bool iIsActive { get; set; }

        public string cAddress { get; set; }

        public string cOUNTCONTACT { get; set; }

        public int DatabaseId { get; set; }

        public string Brokercompany { get; set; }

    }
}
