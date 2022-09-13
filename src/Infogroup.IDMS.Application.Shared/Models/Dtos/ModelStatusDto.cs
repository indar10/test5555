using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Models.Dtos
{
    public class ModelStatusDto
    {
        public int Id { get; set; }

        public int ModelDetailID { get; set; }

        public string iStatus { get; set; }

        public string dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }
    }
}
