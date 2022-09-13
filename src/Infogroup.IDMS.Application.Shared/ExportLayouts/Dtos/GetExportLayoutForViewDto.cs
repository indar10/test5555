using System;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetExportLayoutForViewDto
    {
        public int ID { get; set; }
        public virtual string cDescription { get; set; }

        public virtual bool iHasPhone { get; set; }

        public virtual bool iIsActive { get; set; }

        public virtual DateTime dCreatedDate { get; set; }

        
        public virtual string cCreatedBy { get; set; }

        public virtual DateTime? dModifiedDate { get; set; }

        public virtual string cModifiedBy { get; set; }

        public virtual int? GroupID { get; set; }

        public virtual bool iHasKeyCode { get; set; }

        


        public virtual string cOutputCase { get; set; }
        public virtual string cOutputCaseCode { get; set; }

        public string  cGroupName { get; set; }
        public virtual int? DatabaseId { get; set; }

       


    }
}