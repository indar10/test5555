
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ExternalBuildTableDatabases.Dtos
{
    public class CreateOrEditExternalBuildTableDatabaseDto : EntityDto<int?>
    {
        public int? databaseId { get; set; }
        public int? buildTableId { get; set; }
        public string cCreatedBy { get; set; }
        public string cModifiedBy { get; set; }
        public DateTime? dCreatedDate { get; set; }
        public DateTime? dModifiedDate { get; set; }

    }
}