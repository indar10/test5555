using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infogroup.IDMS.OrderStatuss.Dtos
{
    public class CampaignStatusDto
    {
        public int Id { get; set; }

        public int OrderID { get; set; }

        public int iStatus { get; set; }

        public bool iIsCurrent { get; set; }

        public string dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }

        public string iStopRequested { get; set; }


    }
}
