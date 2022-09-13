using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infogroup.IDMS.IDMSUsers
{
    [Table("tblUser")]
    public class IDMSUser : Entity
    {
        [Required]
        [StringLength(30)]
        public string cFirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string cLastName { get; set; }

        [Required]
        [StringLength(25)]
        public string cUserID { get; set; }

        [Required]
        [StringLength(80)]
        public string cEmail { get; set; }

        [Required]
        [StringLength(20)]
        public string cPhone { get; set; }

        [Required]
        [StringLength(20)]
        public string cFax { get; set; }

        public bool iIsActive { get; set; }

        public bool iIsNotify { get; set; }

        public int iLogonAttempts { get; set; }

        public DateTime? LastLogonDate { get; set; }

        [Required]
        [StringLength(25)]
        public string cCreatedBy { get; set; }

        public DateTime dCreatedDate { get; set; }

        [StringLength(25)]
        public string cModifiedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string LK_AccountingDivisionCode { get; set; }

        public int MailerID { get; set; }

        public int DivisionMailerID { get; set; }

        public int DivisionBrokerID { get; set; }
    }
}
