
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.DivisionMailers.Dtos
{
    public class CreateOrEditDivisionMailerDto : EntityDto<int?>
    {

        public int DivisionId { get; set; }
        public string cCode { get; set; }
        public string cCompany { get; set; }
        public string cFirstName { get; set; }
        public string cLastName { get; set; }
        public string cAddr1 { get; set; }
        public string cAddr2 { get; set; }
        public string cAddr3 { get; set; }
        public string cCity { get; set; }
        public string cState { get; set; }
        public string cZip { get; set; }
        public string cCountry { get; set; }
        public string cPhone { get; set; }
        public string cFax { get; set; }
        public string cEmail { get; set; }
        public string mNotes { get; set; }
        public bool iIsActive { get; set; }
        public DateTime dCreatedDate { get; set; }
        public string cCreatedBy { get; set; }
        public DateTime? dModifiedDate { get; set; }
        public string cModifiedBy { get; set; }

    }
}