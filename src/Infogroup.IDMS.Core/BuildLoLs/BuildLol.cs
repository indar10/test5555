using Infogroup.IDMS.Builds;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.BuildLoLs
{
	[Table("tblBuildLol")]
    public class BuildLol : Entity 
    {

		public virtual int MasterLolID { get; set; }
		
		public virtual string LK_Action { get; set; }
		
		public virtual string LK_ActionMonth1 { get; set; }
		
		public virtual string LK_ActionMonth2 { get; set; }
		
		public virtual string LK_ActionNextMonth { get; set; }
		
		public virtual string LK_QuantityType { get; set; }
		
		public virtual string LK_FileType { get; set; }
		
		public virtual bool iSkipFirstRow { get; set; }
		
		public virtual bool iIsActive { get; set; }
		
		public virtual int iUsage { get; set; }
		
		public virtual int nTurns { get; set; }
		
		public virtual string cDecisionReasoning { get; set; }
		
		public virtual string cSlugDate { get; set; }
		
		public virtual string cBatchDateType { get; set; }
		
		public virtual string LK_SlugDateType { get; set; }
		
		public virtual int iQuantityPrevious { get; set; }
		
		public virtual int iQuantityRequested { get; set; }
		
		public virtual int iQuantityReceivedDP { get; set; }
		
		public virtual int iQuantityReceived { get; set; }
		
		public virtual int iQuantityConverted { get; set; }
		
		public virtual DateTime? dDateReceived { get; set; }
		
		public virtual int iQuantityTotal { get; set; }
		
		public virtual string cBatch_LastFROM { get; set; }
		
		public virtual string cBatch_LastTO { get; set; }
		
		public virtual string cBatch_FROM { get; set; }
		
		public virtual string cBatch_TO { get; set; }
		
		public virtual string Order_No { get; set; }
		
		public virtual string Order_ClientPO { get; set; }
		
		public virtual string OrderSelection { get; set; }
		
		public virtual string Order_Fields { get; set; }
		
		public virtual string Order_Comments { get; set; }
		
		public virtual string Order_Notes1 { get; set; }
		
		public virtual string Order_Notes2 { get; set; }
		
		public virtual string LK_EmailTemplate { get; set; }
		
		public virtual DateTime? ddateOrderSent { get; set; }
		
		public virtual string cNote { get; set; }
		
		public virtual int iCASApprovalTo { get; set; }
		
		public virtual string cSourceFilenameReadyToLoad { get; set; }
		
		public virtual string cSystemFilenameReadyToLoad { get; set; }
		
		public virtual string LK_LoadFileType { get; set; }
		
		public virtual string LK_LoadFileRowTerminator { get; set; }
		
		public virtual string cOnePassFileName { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual string cSQL { get; set; }
		
		public virtual string cSQLDescription { get; set; }
		
		public virtual int iLoadQty { get; set; }
		
		public virtual string LK_Encoding { get; set; }
		
		public virtual bool iIsMultiline { get; set; }
		

		public virtual int BuildId { get; set; }
		
        [ForeignKey("BuildId")]
		public Build BuildFk { get; set; }
		
    }
}