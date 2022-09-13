
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.CampaignDecoys.Dtos
{
    public class CampaignDecoyDto : EntityDto
    {
        
        public string cFirstName { get; set; }

        
        public string cLastName { get; set; }

        
        public string cName { get; set; }

        
        public string cAddress1 { get; set; }

        
        public string cAddress2 { get; set; }

        
        public string cCity { get; set; }

        
        public string cState { get; set; }

        
        public string cZip { get; set; }

        
        public string cCompany { get; set; }

        
        public string cTitle { get; set; }

        
        public string cEmail { get; set; }

        
        public string cPhone { get; set; }

        
        public string cFax { get; set; }

        public DateTime dCreatedDate { get; set; }

        
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

        public string cDecoyType { get; set; }

        public string cZip4 { get; set; }

        public string cKeyCode1 { get; set; }

        public string cDecoyGroup { get; set; }
        public int OrderId { get; set; }

		 
    }
}