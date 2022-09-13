using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}