using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Offers.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Offers
{
    public interface IOffersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetOfferForViewDto>> GetAllOffers(GetAllOffersInput input);

		Task<GetOfferForEditOutput> GetOfferForEdit(EntityDto input);

		void CreateOrEdit(CreateOrEditOfferDto input);

        List<DropdownOutputDto> GetDDForOfferType();

    }
}