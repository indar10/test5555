using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignFavourites
{
	[Table("tblOrderFavorite")]
    public class CampaignFavourite : Entity 
    {
		public virtual int OrderID { get; set; }
		
		public virtual int UserID { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
        [ForeignKey("OrderID")]
		public Campaign Campaign{ get; set; }
		
    }
}