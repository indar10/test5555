using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.AutoSuppresses.Dtos
{
    public class GetAutoSuppressForEditOutput
    {
		public CreateOrEditAutoSuppressDto AutoSuppress { get; set; }

		public string DatabasecDatabaseName { get; set;}


    }
}