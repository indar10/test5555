using System.Threading.Tasks;
using Infogroup.IDMS.Sessions.Dto;

namespace Infogroup.IDMS.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
