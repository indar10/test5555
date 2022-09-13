using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Managers.Dtos
{
    public class ContactAssignmentsDto : EntityDto<int?>
    {

        public string ListManager { get; set; }

        public string ContactName { get; set; }

        public int ContactId{ get; set; }

        public List<string> Dwap { get; set; }

        public List<string> OrderList{ get; set; }

    }
}