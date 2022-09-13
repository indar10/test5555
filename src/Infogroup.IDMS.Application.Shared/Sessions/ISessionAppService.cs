using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.Sessions.Dto;

namespace Infogroup.IDMS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}
