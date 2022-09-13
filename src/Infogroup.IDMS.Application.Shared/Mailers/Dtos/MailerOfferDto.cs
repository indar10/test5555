
namespace Infogroup.IDMS.Mailers.Dtos
{
    public class MailerOfferDto
    {
        public string cOfferCode { get; set; }

        public string cOfferName { get; set; }

        public string LK_OfferType { get; set; }

        public string cCompany { get; set; }

        public bool iIsActive { get; set; }        

        public string iHideInDWAP { get; set; }

        public int MailerId { get; set; }
    }
}
