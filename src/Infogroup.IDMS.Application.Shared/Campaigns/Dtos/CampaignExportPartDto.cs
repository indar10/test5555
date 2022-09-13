
using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.OrderExportParts.Dtos
{
    public class CampaignExportPartDto : EntityDto
    {

        public List<string> cPartNo { get; set; }

        public int SegmentID { get; set; }

        public List<string> iQuantity { get; set; }

        public DateTime dCreatedDate { get; set; }

        
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }


        public int OrderId { get; set; }

        public string SegmentDescription { get; set; }
        
        public int OutputQuantity { get; set; }
        public int ProvidedQuantity { get; set; }

        public int iDedupeOrderSpecified { get; set; }




    }
    public class EditCampaignExportPartDto : EntityDto
    {

        public List<string> cPartNo { get; set; }

        public int SegmentID { get; set; }

        public List<string> iQuantity { get; set; }

        public DateTime dCreatedDate { get; set; }


        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }


        public int OrderId { get; set; }

        public string SegmentDescription { get; set; }

        public int OutputQuantity { get; set; }
        public int ProvidedQuantity { get; set; }
        public int iDedupeOrderSpecified { get; set; }




    }
}