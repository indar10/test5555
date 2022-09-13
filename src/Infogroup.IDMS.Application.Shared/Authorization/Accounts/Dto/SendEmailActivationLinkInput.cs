using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInput
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}