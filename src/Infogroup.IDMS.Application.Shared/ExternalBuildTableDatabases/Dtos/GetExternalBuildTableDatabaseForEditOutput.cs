using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ExternalBuildTableDatabases.Dtos
{
    public class GetExternalBuildTableDatabaseForEditOutput
    {
		public CreateOrEditExternalBuildTableDatabaseDto ExternalBuildTableDatabase { get; set; }


    }
}