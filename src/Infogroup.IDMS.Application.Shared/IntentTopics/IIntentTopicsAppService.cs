using Abp.Application.Services;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.IntentTopics
{
    public interface IIntentTopicsAppService : IApplicationService 
    {
        List<DropdownOutputDto> GetAllIntentTopics();

    }
}