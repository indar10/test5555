using Abp.Domain.Repositories;

namespace Infogroup.IDMS.IDMSUsers
{
    public interface IIDMSUserRepository:IRepository<IDMSUser, int>
    {
        IDMSUser GetUserByUserName(string userName);
    }
}
