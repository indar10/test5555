using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
