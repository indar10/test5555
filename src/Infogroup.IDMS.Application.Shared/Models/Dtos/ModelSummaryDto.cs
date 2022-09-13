using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Models.Dtos
{
    public class ModelSummaryDto
    {
        public int DatabaseId { get; set; }

        public string cModelName { get; set; }

        public int iIntercept { get; set; }

        public string cDescription { get; set; }

        public string cModelNumber { get; set; }

        public string cClientName { get; set; }

        public bool iIsScoredForEveryBuild { get; set; }

        public int nChildTableNumber { get; set; }

        public bool iIsActive { get; set; }

        public DateTime dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

        public string LK_ModelType { get; set; }

        public string LK_GiftWeight { get; set; }
    }
}
