using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ProcessQueues.Dtos
{
    public class GetProcessQueueForEditOutput
    {
		public CreateOrEditProcessQueueDto ProcessQueue { get; set; }


    }
}