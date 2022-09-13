using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ListAutomate.Dtos
{
    public class CreateOrEditIListAutomateDto:EntityDto<int?> 
    {
       // public int ScheduleId { get; set; }        

        public int ListId { get; set; }

        public int BuildId { get; set; }=0;

        public char LK_ListConversionFrequency { get; set; }

        public int iInterval { get; set; }

        public string cScheduleTime { get; set; }

        public string cSystemFileNameReadyToLoad { get; set; }

        public bool iIsActive { get; set; }

        public string cScheduledBy { get; set; }
    }
}