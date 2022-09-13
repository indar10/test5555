using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.BuildTableLayouts.Dtos
{
    public class GetBuildTableLayoutForEditOutput
    {
		public CreateOrEditBuildTableLayoutDto BuildTableLayout { get; set; }

		public string BuildTablecTableName { get; set;}


    }
}