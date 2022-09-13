using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Offers
{
    public interface IOfferRepository : IRepository<Offer, int>
    {
        void UpdateCASApproval(string status, string notes, string ipAddress, string userID, int? offerid);
    }
}
