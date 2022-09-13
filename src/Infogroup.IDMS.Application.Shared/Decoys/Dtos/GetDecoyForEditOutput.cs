using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Decoys.Dtos
{
    public class GetDecoyForEditOutput
    {
		public CreateOrEditDecoyDto Decoy { get; set; }

		public string DatabasecDatabaseName { get; set;}


    }
}