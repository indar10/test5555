using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Managers.Dtos
{
    public class CreateOrEditManagerDto : EntityDto<int?>
    {

        public string cCode { get; set; }

        public string cCompany { get; set; }

        public string cAddress1 { get; set; }

        public string cAddress2 { get; set; }

        public string cCity { get; set; }

        public string cState { get; set; }

        public string cZip { get; set; }

        public string cPhone { get; set; }

        public string cFax { get; set; }

        public string cNotes { get; set; }

        public bool iIsActive { get; set; }

        public int DatabaseId { get; set; }

        public string cCreatedBy { get; set; }

        public string cModifiedBy { get; set; }

        public DateTime dCreatedDate { get; set; }

        public DateTime? dModifiedDate { get; set; }


    }
}