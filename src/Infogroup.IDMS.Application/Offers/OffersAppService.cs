using Infogroup.IDMS.Mailers;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Offers.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Sessions;
using Abp.UI;
using System.Collections.Generic;

namespace Infogroup.IDMS.Offers
{
    [AbpAuthorize(AppPermissions.Pages_Offers)]
    public class OffersAppService : IDMSAppServiceBase, IOffersAppService
    {
        private readonly IRepository<Mailer, int> _mailerRepository;
        private readonly IRepository<Lookup, int> _lookupRepository;
        private readonly IOfferRepository _customOfferRepository;
        private readonly AppSession _mySession;
        private readonly OfferDomainService _offerDomainService;

        public OffersAppService(IRepository<Mailer, int> mailerRepository,
            IRepository<Lookup, int> lookupRepository,
            IOfferRepository customOfferRepository,
            AppSession mySession,
            OfferDomainService offerDomainService)
        {
            _mailerRepository = mailerRepository;
            _lookupRepository = lookupRepository;
            _customOfferRepository = customOfferRepository;
            _mySession = mySession;
            _offerDomainService = offerDomainService;
        }

        #region Fetch Offers
        public async Task<PagedResultDto<GetOfferForViewDto>> GetAllOffers(GetAllOffersInput input)
        {
            try
            {
                var filteredOffers = _customOfferRepository.GetAll()
                            .WhereIf(input.MailerId > -1, e => Convert.ToInt32(e.MailerId) == input.MailerId);
                var totalCount = await filteredOffers.CountAsync();


                var pagedAndFilteredOffers = filteredOffers
                    .OrderBy(input.Sorting ?? OfferConsts.IdAsc)
                    .PageBy(input);

                var offers = from o in pagedAndFilteredOffers
                             join o1 in _mailerRepository.GetAll() on o.MailerId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()

                             select new GetOfferForViewDto
                             {
                                 Offer = ObjectMapper.Map<OfferDto>(o),
                                 MailercCompany = s1 == null ? string.Empty : s1.cCompany.ToString()
                             };

                return new PagedResultDto<GetOfferForViewDto>(
                    totalCount,
                    await offers.ToListAsync()
                );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<GetOfferForEditOutput> GetOfferForEdit(EntityDto input)
        {
            try
            {
                var offer = await _customOfferRepository.FirstOrDefaultAsync(input.Id);
                var output = new GetOfferForEditOutput { Offer = ObjectMapper.Map<CreateOrEditOfferDto>(offer) };

                output.Offer.OfferTypeDescription = GetDDForOfferType();

                var mailer = await _mailerRepository.FirstOrDefaultAsync(output.Offer.MailerId);
                output.MailercCompany = mailer.cCompany;

                return output;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Save Offers
        [AbpAuthorize(AppPermissions.Pages_Offers_Create, AppPermissions.Pages_Offers_Edit)]
        public void CreateOrEdit(CreateOrEditOfferDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var offer = ObjectMapper.Map<Offer>(input);
                    _offerDomainService.AddOffer(offer, input.isAutoApprove);
                }
                else
                {
                    var updateOffer = _customOfferRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateOffer);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        public List<DropdownOutputDto> GetDDForOfferType()
        {
            try
            {
                return _lookupRepository.GetAll().Where(p => p.cLookupValue == OfferConsts.OfferType).
                    Select(p => new DropdownOutputDto { Label = p.cDescription, Value = p.cCode }).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}