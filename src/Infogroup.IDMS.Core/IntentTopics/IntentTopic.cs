using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.IntentTopics
{
	[Table("tblIntentTopic")]
    public class IntentTopic : Entity 
    {

		[Required]
		public virtual string cTopic { get; set; }
		

    }
}