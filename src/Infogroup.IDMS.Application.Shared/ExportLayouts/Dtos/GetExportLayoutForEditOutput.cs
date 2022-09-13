using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetExportLayoutForEditOutput
    {
		public CreateOrEditExportLayoutDto ExportLayout { get; set; }

		public string DatabasecDatabaseName { get; set;}


    }
}