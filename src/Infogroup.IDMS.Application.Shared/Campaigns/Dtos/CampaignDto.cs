
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class CampaignDto : EntityDto
    {
		public int ID { get; set; }

		public int BuildID { get; set; }

		public int MailerID { get; set; }

		public int OfferID { get; set; }

		public string cOrderType { get; set; }

		public int UserID { get; set; }

		public bool iIsOrder { get; set; }

		public string cDescription { get; set; }

		public int iProvidedCount { get; set; }

		public bool iIsRandomExecution { get; set; }

		public bool iIsNetUse { get; set; }

		public DateTime? dDateLastRun { get; set; }

		public string cSortFields { get; set; }

		public string cOrderNo { get; set; }

		public string cLVAOrderNo { get; set; }

		public string cNextMarkOrderNo { get; set; }

		public string cBrokerPONo { get; set; }

		public string cNotes { get; set; }

		public int iDecoyQty { get; set; }

		public int iDecoyKeyMethod { get; set; }

		public string cDecoyKey { get; set; }

		public bool cDecoysByKeycode { get; set; }

		public string cSpecialProcess { get; set; }

		public string cShiptoType { get; set; }

		public string cShipTOEmail { get; set; }

		public string cShipCCEmail { get; set; }

		public string cShipSUBJECT { get; set; }

		public DateTime? dShipDateShipped { get; set; }

		public string LK_ExportFileFormatID { get; set; }

		public bool iIsAddHeader { get; set; }

		public string cDatabaseName { get; set; }

		public int? iSplitType { get; set; }

		public int? iSplitIntoNParts { get; set; }

		public string cExportLayout { get; set; }

		public bool iIsNoUsage { get; set; }

		public bool iHouseFilePriority { get; set; }

		public string cOutputCase { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public DateTime? dModifiedDate { get; set; }

		public string cModifiedBy { get; set; }

		public string cOfferName { get; set; }

		public int DivisionMailerID { get; set; }

		public int DivisionBrokerID { get; set; }

		public string cDecoyKey1 { get; set; }

		public int? iAvailableQty { get; set; }

		public DateTime? dMailDate { get; set; }

		public int? iMailDatePlus { get; set; }

		public int? iMailDateMinus { get; set; }

		public int iMinQuantityOrderLevelMaxPer { get; set; }

		public int iMaxQuantityOrderLevelMaxPer { get; set; }

		public string cMaxPerFieldOrderLevel { get; set; }

		public string cChannelType { get; set; }

		public string cFileLabel { get; set; }

		public string cSANNumber { get; set; }

		public bool iIsExportDataFileOnly { get; set; }



    }
}