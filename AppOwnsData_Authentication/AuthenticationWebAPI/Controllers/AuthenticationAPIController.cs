using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Web.Http;
namespace AuthenticationWebAPI.Controllers
{
    public class AuthenticationAPIController : ApiController
    {      
        [HttpGet]
        public async Task<string> Get(string AuthorityUrl, string ApplicationId, string ResourceUrl, string Username, string Password)
        {
            var credential = new UserPasswordCredential(Username, Password);
            var authenticationContext = new AuthenticationContext(AuthorityUrl);
            var authenticationResult = await authenticationContext.AcquireTokenAsync(ResourceUrl, ApplicationId, credential);
            return authenticationResult.AccessToken;
        }      
    }
}
