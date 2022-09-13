using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Campaigns
{
	[Table("tblOrder")]
    public class Campaign : Entity 
    {
        public int BuildID { get; set; }

        public int MailerID { get; set; }

        public int OfferID { get; set; }

        [Required]
        [StringLength(1)]
        public string cOrderType { get; set; }

        public int UserID { get; set; }

        public bool iIsOrder { get; set; }

        [Required]
        [StringLength(50)]
        public string cDescription { get; set; }

        public int iProvidedCount { get; set; }

        public bool iIsRandomExecution { get; set; }

        public bool iIsNetUse { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? dDateLastRun { get; set; }

        [Required]
        [StringLength(50)]
        public string cSortFields { get; set; }

        [Required]
        [StringLength(50)]
        public string cOrderNo { get; set; }

        [StringLength(25)]
        public string cLVAOrderNo { get; set; }

        [Required]
        [StringLength(15)]
        public string cNextMarkOrderNo { get; set; }

        [StringLength(30)]
        public string cBrokerPONo { get; set; }

        public string cNotes { get; set; }

        public int iDecoyQty { get; set; }

        public int iDecoyKeyMethod { get; set; }

        [Required]
        [StringLength(10)]
        public string cDecoyKey { get; set; }

        public bool cDecoysByKeycode { get; set; }

        public string cSpecialProcess { get; set; }

        [Required]
        [StringLength(1)]
        public string cShiptoType { get; set; }

        public string cShipTOEmail { get; set; }

        public string cShipCCEmail { get; set; }

        [Required]
        [StringLength(200)]
        public string cShipSUBJECT { get; set; }

        public DateTime? dShipDateShipped { get; set; }

        [StringLength(3)]
        public string LK_ExportFileFormatID { get; set; }

        public bool? iIsAddHeader { get; set; }

        public bool? iIsUncompressed { get; set; }

        public string LK_PGPKeyFile { get; set; }

        [StringLength(80)]
        public string cDatabaseName { get; set; }

        public int? iSplitType { get; set; }

        public int? iSplitIntoNParts { get; set; }

        [StringLength(50)]
        public string cExportLayout { get; set; }

        public bool? iIsNoUsage { get; set; }

        public bool? iHouseFilePriority { get; set; }

        public string cOutputCase { get; set; }

        public DateTime dCreatedDate { get; set; }

        [Required]
        [StringLength(25)]
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        [StringLength(25)]
        public string cModifiedBy { get; set; }

        [Required]
        [StringLength(50)]
        public string cOfferName { get; set; }

        public int DivisionMailerID { get; set; }

        public int DivisionBrokerID { get; set; }

        [StringLength(25)]
        public string cDecoyKey1 { get; set; }

        public int? iAvailableQty { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? dMailDate { get; set; }

        public int? iMailDatePlus { get; set; }

        public int? iMailDateMinus { get; set; }

        public int iMinQuantityOrderLevelMaxPer { get; set; }

        public int iMaxQuantityOrderLevelMaxPer { get; set; }

        [Required]
        [StringLength(50)]
        public string cMaxPerFieldOrderLevel { get; set; }

        [StringLength(1)]
        public string cChannelType { get; set; }

        [Required]
        [StringLength(50)]
        public string cFileLabel { get; set; }

        [StringLength(30)]
        public string cSANNumber { get; set; }

        public bool? iIsExportDataFileOnly { get; set; }
        [StringLength(2)]
        public string LK_Media { get; set; }

        [StringLength(10)]
        public string LK_AccountCode { get; set; }

        [StringLength(50)]
        public string cBillingCompany { get; set; }

        [StringLength(20)]
        public string cBillingPhone { get; set; }

        public int? iExportLayoutID { get; set; }
        
    }
}