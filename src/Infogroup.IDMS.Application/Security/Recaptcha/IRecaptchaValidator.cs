using System.Threading.Tasks;

namespace Infogroup.IDMS.Security.Recaptcha
{
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}