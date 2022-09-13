using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.ListMailerRequesteds
{
    public interface IListMailerRequestedsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetListMailerRequestedForViewDto>> GetAll(GetAllListMailerRequestedsInput input);

        Task<GetListMailerRequestedForViewDto> GetListMailerRequestedForView(int id);

		Task<GetListMailerRequestedForEditOutput> GetListMailerRequestedForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditListMailerRequestedDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetListMailerRequestedsToExcel(GetAllListMailerRequestedsForExcelInput input);

		
		Task<PagedResultDto<ListMailerRequestedMasterLoLLookupTableDto>> GetAllMasterLoLForLookupTable(GetAllForLookupTableInput input);
		
    }
}