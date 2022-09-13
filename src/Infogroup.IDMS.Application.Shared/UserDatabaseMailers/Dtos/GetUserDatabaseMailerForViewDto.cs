using System;

namespace Infogroup.IDMS.UserDatabaseMailers.Dtos
{
    public class GetUserDatabaseMailerForViewDto
    {
        public virtual int Count { get; set; }
        public virtual int MailerID { get; set; }       
        public virtual string cCreatedBy { get; set; }
        public virtual DateTime dCreatedDate { get; set; }
        public virtual string cModifiedBy { get; set; }
        public virtual DateTime? dModifiedDate { get; set; }
        public virtual int UserId { get; set; }   

        public virtual int DatabaseId { get; set; }

        
       


    }
}