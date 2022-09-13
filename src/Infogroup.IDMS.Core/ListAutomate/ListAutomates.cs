using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ListAutomate
{
    [Table("tblListConversionSchedule")]
    public class ListAutomates : Entity
    {
       // public virtual int ScheduleId { get; set; }

        public virtual int ListId { get; set; }

        public virtual int BuildId { get; set; }

        public virtual string LK_ListConversionFrequency { get; set; }

        public virtual int iInterval { get; set; }

        public virtual string cScheduleTime { get; set; }

        public virtual string cScheduledBy { get; set; }

        public virtual string cSystemFileNameReadyToLoad { get; set; }

        public virtual bool iIsActive { get; set; }

    }
}