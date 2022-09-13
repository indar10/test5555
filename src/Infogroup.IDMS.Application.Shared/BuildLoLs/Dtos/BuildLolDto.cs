
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.BuildLoLs.Dtos
{
    public class BuildLolDto : EntityDto
    {
		public int MasterLolID { get; set; }

		public string LK_Action { get; set; }

		public string LK_ActionMonth1 { get; set; }

		public string LK_ActionMonth2 { get; set; }

		public string LK_ActionNextMonth { get; set; }

		public string LK_QuantityType { get; set; }

		public string LK_FileType { get; set; }

		public bool iSkipFirstRow { get; set; }

		public bool iIsActive { get; set; }

		public int iUsage { get; set; }

		public int nTurns { get; set; }

		public string cDecisionReasoning { get; set; }

		public string cSlugDate { get; set; }

		public string cBatchDateType { get; set; }

		public string LK_SlugDateType { get; set; }

		public int iQuantityPrevious { get; set; }

		public int iQuantityRequested { get; set; }

		public int iQuantityReceivedDP { get; set; }

		public int iQuantityReceived { get; set; }

		public int iQuantityConverted { get; set; }

		public DateTime? dDateReceived { get; set; }

		public int iQuantityTotal { get; set; }

		public string cBatch_LastFROM { get; set; }

		public string cBatch_LastTO { get; set; }

		public string cBatch_FROM { get; set; }

		public string cBatch_TO { get; set; }

		public string Order_No { get; set; }

		public string Order_ClientPO { get; set; }

		public string OrderSelection { get; set; }

		public string Order_Fields { get; set; }

		public string Order_Comments { get; set; }

		public string Order_Notes1 { get; set; }

		public string Order_Notes2 { get; set; }

		public string LK_EmailTemplate { get; set; }

		public DateTime? ddateOrderSent { get; set; }

		public string cNote { get; set; }

		public int iCASApprovalTo { get; set; }

		public string cSourceFilenameReadyToLoad { get; set; }

		public string cSystemFilenameReadyToLoad { get; set; }

		public string LK_LoadFileType { get; set; }

		public string LK_LoadFileRowTerminator { get; set; }

		public string cOnePassFileName { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public DateTime? dModifiedDate { get; set; }

		public string cModifiedBy { get; set; }

		public string cSQL { get; set; }

		public string cSQLDescription { get; set; }

		public int iLoadQty { get; set; }

		public string LK_Encoding { get; set; }

		public bool iIsMultiline { get; set; }


		 public int BuildId { get; set; }

		 
    }
}