using Infogroup.IDMS.SecurityGroups;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.GroupBrokers
{
	[Table("tblGroupBroker")]
    public class GroupBroker : Entity 
    {

		public virtual int BrokerID { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

		public virtual int GroupID { get; set; }
		
        [ForeignKey("GroupID")]
		public SecurityGroup GroupFk { get; set; }
		
    }
}