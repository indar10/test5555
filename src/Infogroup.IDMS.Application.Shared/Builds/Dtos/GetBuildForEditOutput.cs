using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Builds.Dtos
{
    public class GetBuildForEditOutput
    {
		public CreateOrEditBuildDto Build { get; set; }

		public string DatabasecDatabaseName { get; set;}


    }
}