using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.OfferSamples.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.OfferSamples
{
    public interface IOfferSamplesAppService : IApplicationService 
    {
        Task<PagedResultDto<OfferSampleDto>> GetAllOfferSamples(GetAllOfferSamplesInput input);

		Task<CreateOrEditOfferSampleDto> GetOfferSampleForEdit(EntityDto input);

        void CreateOrEdit(CreateOrEditOfferSampleDto input, string clientFileName, string path);

    }
}