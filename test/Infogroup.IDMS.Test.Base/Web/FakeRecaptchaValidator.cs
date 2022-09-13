using System.Threading.Tasks;
using Infogroup.IDMS.Security.Recaptcha;

namespace Infogroup.IDMS.Test.Base.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}
