using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ExternalBuildTableDatabases.Dtos
{
   public class ExternalBuildTableDatabaseForAllDto : PagedAndSortedResultRequestDto
    {
        public int ID { get; set; }
        public int DivisionID { get; set; }
        public int DatabaseID { get; set; }
        public int BuildTableID { get; set; }
        public string DivisionName { get; set; }
        public string DatabaseName { get; set; }
        public string BuildTableDescription { get; set; }
        public string BuildTableName { get; set; }
    }
}
