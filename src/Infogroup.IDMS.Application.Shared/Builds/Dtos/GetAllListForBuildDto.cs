using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Builds.Dtos
{
    public class GetAllListForBuildDto
    {
        public int ID { get; set; }

        public int BuildID { get; set; }

        public int MasterLolID { get; set; }

        public char LK_Action { get; set; }

        public string cSlugDate { get; set; }

        public char cBatchDateType { get; set; }

        public char LK_SlugDateType { get; set; }

        public int iQuantityTotal { get; set; }

        public string cOnePassFileName { get; set; }

        public DateTime dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }


    }
}
