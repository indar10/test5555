using Infogroup.IDMS.Databases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.MasterLoLs
{
	[Table("tblMasterLoL")]
    public class MasterLoL : Entity 
    {

		public virtual int OwnerID { get; set; }
		
		public virtual int ManagerID { get; set; }
		
		[Required]
		public virtual string LK_PermissionType { get; set; }
		
		[Required]
		public virtual string LK_ListType { get; set; }
		
		[Required]
		public virtual string LK_ProductCode { get; set; }
		
		[Required]
		public virtual string LK_DecisionGroup { get; set; }
		
		[Required]
		public virtual string cCode { get; set; }
		
		public virtual string cDMIDWListNumber { get; set; }
		
		[Required]
		public virtual string cListName { get; set; }
		
		[Required]
		public virtual string cMINDatacardCode { get; set; }
		
		[Required]
		public virtual string cNextMarkID { get; set; }
		
		public virtual int iOrderContactID { get; set; }
		
		public virtual bool iIsMultibuyer { get; set; }
		
		[Required]
		public virtual string cAppearDate { get; set; }
		
		[Required]
		public virtual string cLastUpdateDate { get; set; }
		
		[Required]
		public virtual string cRemoveDate { get; set; }
		
		public virtual bool iSendCASApproval { get; set; }
		
		public virtual DateTime? dEmailSentDate { get; set; }
		
		public virtual int nBasePrice_Postal { get; set; }
		
		public virtual int nBasePrice_Telemarketing { get; set; }
		
		public virtual int? nNewBasePrice_Postal { get; set; }
		
		public virtual int? nNewBasePrice_Telemarketing { get; set; }
		
		public virtual string cCAS_Signature { get; set; }
		
		public virtual string cCAS_IPAddress { get; set; }
		
		public virtual string cCAS_ApprovedBy { get; set; }
		
		public virtual bool iIsActive { get; set; }
		
		public virtual bool iApprovalForm { get; set; }
		
		public virtual bool iDropDuplicates { get; set; }
		
		[Required]
		public virtual string cCustomText1 { get; set; }
		
		[Required]
		public virtual string cCustomText2 { get; set; }
		
		[Required]
		public virtual string cCustomText3 { get; set; }
		
		[Required]
		public virtual string cCustomText4 { get; set; }
		
		[Required]
		public virtual string cCustomText5 { get; set; }
		
		[Required]
		public virtual string cCustomText6 { get; set; }
		
		[Required]
		public virtual string cCustomText7 { get; set; }
		
		public virtual string cCustomText8 { get; set; }
		
		public virtual string cCustomText9 { get; set; }
		
		public virtual string cCustomText10 { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual bool iIsNCOARequired { get; set; }
		
		public virtual bool iIsProfanityCheckRequired { get; set; }
		
		public virtual Guid? GUID { get; set; }
		
		public virtual DateTime? dValidUpTill { get; set; }
		

		public virtual int DatabaseId { get; set; }
		
        [ForeignKey("DatabaseId")]
		public Database DatabaseFk { get; set; }
		
    }
}