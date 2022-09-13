using Abp.Domain.Services;
using Infogroup.IDMS.Mailers;
using Microsoft.AspNetCore.Http;

namespace Infogroup.IDMS.Offers
{
    public class OfferDomainService : DomainService
    {
        private readonly IOfferRepository _customOfferRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OfferDomainService(IOfferRepository customOfferRepository,
              IHttpContextAccessor httpContextAccessor)
        {
            _customOfferRepository = customOfferRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public void AddOffer(Offer offer, bool isAutoApprove)
        {

            var offerId = _customOfferRepository.InsertAndGetId(offer);
            if (isAutoApprove)
            {
                var ipAddress = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress?.ToString();
                _customOfferRepository.UpdateCASApproval(MailerConsts.AutoApprovedStatus, MailerConsts.Notes, ipAddress, offer.cCreatedBy, offerId);
            }
        }
    }
}

