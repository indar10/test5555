using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.BatchQueues.Dtos
{
    public class BatchQueueDto : EntityDto
    {
        public int ID { get; set; }

        public int iStatusId { get; set; }

        public string FieldName { get; set; }

        public string ParmData { get; set; }

        public string cScheduleBy { get; set; }

        public DateTime dScheduled { get; set; }

        public string BuildDescription { get; set; }

        public string ProcessTypeDescription { get; set; }

        public int ProcessTypeId { get; set; }

        public string DataBaseName { get; set; }

        public string StatusDescription { get; set; }

        public string Result { get; set; }

        public string duration { get; set; }





    }
}