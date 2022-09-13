using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ExportLayoutDetails.Dtos
{
    public class GetExportLayoutDetailForEditOutput
    {
		public CreateOrEditExportLayoutDetailDto ExportLayoutDetail { get; set; }

		public string ExportLayoutcDescription { get; set;}


    }
}