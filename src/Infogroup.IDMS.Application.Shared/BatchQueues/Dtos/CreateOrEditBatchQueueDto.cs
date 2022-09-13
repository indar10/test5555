
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.BatchQueues.Dtos
{
    public class CreateOrEditBatchQueueDto : EntityDto<int?>
    {

        //public int ID { get; set; }

        
        public int  iStatusId { get; set; }

         public string ProcessTypeDescription { get; set; }

        public int ProcessTypeId { get; set; }

         public string Result { get; set; }

       public DateTime? dEndDate { get; set; }

    }
}