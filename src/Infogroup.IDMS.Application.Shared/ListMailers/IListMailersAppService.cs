using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.ListMailers
{
    public interface IListMailersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetListMailerForViewDto>> GetAll(GetAllListMailersInput input);

  //      Task<GetListMailerForViewDto> GetListMailerForView(int id);

		//Task<GetListMailerForEditOutput> GetListMailerForEdit(EntityDto input);

		//Task CreateOrEdit(CreateOrEditListMailerDto input);

		//Task Delete(EntityDto input);

		//Task<FileDto> GetListMailersToExcel(GetAllListMailersForExcelInput input);

		
		//Task<PagedResultDto<ListMailerMasterLoLLookupTableDto>> GetAllMasterLoLForLookupTable(GetAllForLookupTableInput input);
		
    }
}