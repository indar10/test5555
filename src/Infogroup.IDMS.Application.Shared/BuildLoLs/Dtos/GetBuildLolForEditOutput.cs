using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.BuildLoLs.Dtos
{
    public class GetBuildLolForEditOutput
    {
		public CreateOrEditBuildLolDto BuildLol { get; set; }

		public string BuildLK_BuildStatus { get; set;}


    }
}