using Infogroup.IDMS.Authorization.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Infogroup.IDMS.AbpUserPasswords
{
	[Table("tblAbpUserPasswords")]
    public class AbpUserPassword : CreationAuditedEntity<long>
    {
		[Required]
		[StringLength(AbpUserPasswordConsts.MaxPasswordLength, MinimumLength = AbpUserPasswordConsts.MinPasswordLength)]
		public virtual string Password { get; set; }		
		[Required]
		public virtual bool IsActive { get; set; }		
		public virtual long UserId { get; set; }		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }		
    }
}