using Abp.Application.Services;
using Infogroup.IDMS.UserSavedSelections.Dtos;
using System.Threading.Tasks;

namespace Infogroup.IDMS.UserSavedSelections
{
    public interface IUserSavedSelectionsAppService : IApplicationService
    {
        Task<string> Create(int sourceSegment, CreateOrEditUserSavedSelectionDto input);
    }
}