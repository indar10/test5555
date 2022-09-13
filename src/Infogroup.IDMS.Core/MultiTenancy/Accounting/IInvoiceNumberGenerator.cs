using System.Threading.Tasks;
using Abp.Dependency;

namespace Infogroup.IDMS.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}