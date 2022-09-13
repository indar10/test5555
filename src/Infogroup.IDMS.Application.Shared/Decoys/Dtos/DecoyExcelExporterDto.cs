using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Decoys.Dtos
{
    public class DecoyExcelExporterDto : EntityDto
    {
        public string cCode { get; set; }

        public string cCompany { get; set; }

        public bool iIsActive { get; set; }

        public string cAddress { get; set; }

        public int DatabaseId { get; set; }

        public int ContactsCount { get; set; }

        public int DecoysCount { get; set; }

        public string cAddress1 { get; set; }

        public string cAddress2 { get; set; }

        public string cCity { get; set; }

        public string cState { get; set; }

        public string cZip { get; set; }

        public string cPhone { get; set; }

        public string cFax { get; set; }

        public string broker { get; set; }

        public List<DecoyDto> DecoysList { get; set; }
    }
}
