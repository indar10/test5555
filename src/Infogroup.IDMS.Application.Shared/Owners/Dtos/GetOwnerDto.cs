using System;
using System.Collections.Generic;
using System.Text;
using Infogroup.IDMS.Databases;

namespace Infogroup.IDMS.Owners.Dtos
{
    public class GetOwnerDto
    {
        public virtual string cCode { get; set; }

        public virtual string cCompany { get; set; }

        public virtual string cAddress1 { get; set; }

        public virtual string cAddress2 { get; set; }

        public virtual string cCity { get; set; }

        public virtual string cState { get; set; }

        public virtual string cZip { get; set; }

        public virtual string cPhone { get; set; }

        public virtual string cFax { get; set; }

        public virtual string cNotes { get; set; }

        public virtual bool iIsActive { get; set; }

        public virtual DateTime dCreatedDate { get; set; }

        public virtual string cCreatedBy { get; set; }

        public virtual string cModifiedBy { get; set; }

        public virtual DateTime? dModifiedDate { get; set; }


        public virtual int DatabaseId { get; set; }
        public int DatabaseFk { get; set; }
       
    }
}
