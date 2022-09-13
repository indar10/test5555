using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSConfigurations.Dtos
{
    public class GetAllConfigurationsForViewDto: PagedAndSortedResultRequestDto
    {
        public int ID { get; set; }
        public int DivisionID { get; set; }
        public int DatabaseID { get; set; }
        public string cItem { get; set; }
        public string cDescription { get; set; }
        public string cValue { get; set; }
        public bool iIsActive { get; set; }
        public string cDatabaseName { get; set; }
    }
}