using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.MatchAppends.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infogroup.IDMS.MatchAppends
{
    public interface IMatchAppendsAppService : IApplicationService 
    {
        PagedResultDto<GetMatchAppendForViewDto> GetAllMatchAppendTasks(GetAllMatchAppendsInput input);

        void SubmitUnlockMatchAppendTask(int matchAppendId, bool isSubmit);
        List<MatchAndAppendInputLayoutDto> ImportFielList(string importFieldString, int matchAppendId, CreateOrEditMatchAppendDto matchAppendDto);
        List<DropdownOutputDto> GetMatchAppendDatabasesBasedOnUserId(int userId);

        Task<GetMatchAppendForEditOutput> GetMatchAppendForEdit(int matchAppendId);       

        void CreateOrEdit(CreateOrEditMatchAppendDto input);
        void CreateEditAndSubmit(CreateOrEditMatchAppendDto input);





    }
}