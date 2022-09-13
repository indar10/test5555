using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ProcessQueueDatabases.Dtos
{
    public class GetProcessQueueDatabaseForEditOutput
    {
		public CreateOrEditProcessQueueDatabaseDto ProcessQueueDatabase { get; set; }

		public string ProcessQueuecQueueName { get; set;}


    }
}