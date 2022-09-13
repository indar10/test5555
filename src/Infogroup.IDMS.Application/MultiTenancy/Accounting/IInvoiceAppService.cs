using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.MultiTenancy.Accounting.Dto;

namespace Infogroup.IDMS.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
