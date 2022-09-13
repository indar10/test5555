using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSConfigurations.Dtos
{
    public class GetAllConfigurationsListInput: PagedAndSortedResultRequestDto
    {    
        public string Filter { get; set; }
        //public int iIsActiveFilter { get; set; }        
        //public string Description { get; set; }
        //public string DatabaseName { get; set; }
        //public string DatabaseName1 { get; set; }
        //public string Item { get; set; }
        //public string Item1 { get; set; }
        //public string DivisionName { get; set; }
        //public string Value { get; set; }
    }
}
