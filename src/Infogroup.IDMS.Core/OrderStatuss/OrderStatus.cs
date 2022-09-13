using Abp.Domain.Entities;
using Infogroup.IDMS.Campaigns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infogroup.IDMS.OrderStatuss
{
    [Table("tblOrderStatus")]
    public class OrderStatus : Entity
    {
        public int OrderID { get; set; }

        public int iStatus { get; set; }

        public bool iIsCurrent { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string cNotes { get; set; }

        public DateTime dCreatedDate { get; set; }

        [Required]
        [StringLength(25)]
        public string cCreatedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? dModifiedDate { get; set; }

        [StringLength(25)]
        public string cModifiedBy { get; set; }

        public bool? iStopRequested { get; set; }

        public virtual Campaign Order { get; set; }
    }
}
