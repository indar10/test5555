
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ExternalBuildTableDatabases.Dtos
{
    public class ExternalBuildTableDatabaseDto : EntityDto
    {
        public int? DatabaseID { get; set; }

        public int? BuildTableID { get; set; }

        public DateTime? dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

    }
}