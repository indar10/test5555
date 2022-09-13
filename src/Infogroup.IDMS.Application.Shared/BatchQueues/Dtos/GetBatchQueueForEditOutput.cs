using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.BatchQueues.Dtos
{
    public class GetBatchQueueForEditOutput
    {
		public CreateOrEditBatchQueueDto BatchQueue { get; set; }


    }
}