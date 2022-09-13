using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
