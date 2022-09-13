
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.IDMSUsers.Dtos
{
    public class IDMSUserDto : EntityDto
    {
        
        public  string cFirstName { get; set; }

        
        public  string cLastName { get; set; }

        
        public  string cUserID { get; set; }

        
        public  string cEmail { get; set; }

        
        public  string cPhone { get; set; }

        
        public  string cFax { get; set; }

        public  bool iIsActive { get; set; }

        public  bool iIsNotify { get; set; }

        public  int iLogonAttempts { get; set; }

        public  DateTime? LastLogonDate { get; set; }

        
        public  string cCreatedBy { get; set; }

        public  DateTime dCreatedDate { get; set; }

        public  string cModifiedBy { get; set; }

        public  DateTime? dModifiedDate { get; set; }

        
        public  string LK_AccountingDivisionCode { get; set; }

        public  int? DivisionBrokerID { get; set; }

        public  int? DivisionMailerID { get; set; }

        public  int? MailerID { get; set; }

    }
}