using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SysSendMails
{
	[Table("sysSendMails")]
    public class SysSendMail : Entity 
    {

		[Required]
		public virtual string cRecipients { get; set; }
		
		[Required]
		public virtual string cMessage { get; set; }
		
		public virtual string cFrom { get; set; }
		
		public virtual string cReplyTo { get; set; }
		
		[Required]
		public virtual string cCopyRecipients { get; set; }
		
		[Required]
		public virtual string cBlindCopyRecipients { get; set; }
		
		[Required]
		public virtual string cSubject { get; set; }
		
		public virtual string cFileName { get; set; }
		
		public virtual int iMailSent { get; set; }
		
		public virtual int iSendFailed { get; set; }
		
		[Required]
		public virtual string cFailureMessage { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual DateTime? dDateSent { get; set; }
		

    }
}