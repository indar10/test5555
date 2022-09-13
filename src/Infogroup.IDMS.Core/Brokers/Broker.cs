using Abp.Domain.Entities;
using Infogroup.IDMS.Databases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infogroup.IDMS.Brokers
{
    [Table("tblBroker")]
    public class Broker : Entity
    {
        public virtual int DatabaseID { get; set; }

        [Required]
        [StringLength(15)]
        public virtual string cCode { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string cCompany { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string cAddress1 { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string cAddress2 { get; set; }

        [Required]
        [StringLength(30)]
        public virtual string cCity { get; set; }

        [Required]
        [StringLength(2)]
        public virtual string cState { get; set; }

        [Required]
        [StringLength(10)]
        public virtual string cZip { get; set; }

        [Required]
        [StringLength(20)]
        public virtual string cPhone { get; set; }

        [Required]
        [StringLength(20)]
        public virtual string cFax { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public virtual string cNotes { get; set; }

        public virtual bool iIsActive { get; set; }

        public virtual DateTime dCreatedDate { get; set; }

        [Required]
        [StringLength(25)]
        public virtual string cCreatedBy { get; set; }

        public virtual DateTime? dModifiedDate { get; set; }

        [StringLength(25)]
        public virtual string cModifiedBy { get; set; }

        public virtual Database Database { get; set; }
    }
}
