using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Decoys.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Mailers.Dtos;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Decoys
{
    public interface IDecoysAppService : IApplicationService 
    {
		PagedResultDto<MailerDto> GetAllDecoyMailers(GetAllDecoysInput input);

		Task<CreateOrEditDecoyDto> GetDecoyForEdit(EntityDto input);
		List<DropdownOutputDto> FillGroupsForEdit();

		Task CreateOrEditDecoy(CreateOrEditDecoyDto input);
		void CopyDecoy(int input);

		Task DeleteDecoy(int input);

		FileDto ExportToExcel(GetAllDecoysInput input);
	}
}