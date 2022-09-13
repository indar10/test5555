using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ExternalBuildTableDatabases.Dtos
{
    public class GetAllExternalBuildTableDatabasesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}