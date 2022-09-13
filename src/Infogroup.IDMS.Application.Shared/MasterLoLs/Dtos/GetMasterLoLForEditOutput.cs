using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.MasterLoLs.Dtos
{
    public class GetMasterLoLForEditOutput
    {
		public CreateOrEditMasterLoLDto MasterLoL { get; set; }

		public string DatabasecDatabaseName { get; set;}


    }
}