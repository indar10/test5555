using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.BuildTables.Dtos
{
    public class GetBuildTableForEditOutput
    {
		public CreateOrEditBuildTableDto BuildTable { get; set; }

		public string BuildcBuild { get; set;}


    }
}