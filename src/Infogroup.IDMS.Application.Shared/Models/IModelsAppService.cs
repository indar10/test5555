using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Models.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.Models
{
    public interface IModelsAppService : IApplicationService 
    {
        PagedResultDto<ModelScoringDto> GetAllModels(GetAllModelsInput input);

        CreateOrEditModelDto GetModelForEdit(int modelDetailId, bool isCopy);

        Task CreateAsync(CreateOrEditModelDto input);

        GetModelTypeAndWeightDto GetModelTypeAndWeight();

    }
}