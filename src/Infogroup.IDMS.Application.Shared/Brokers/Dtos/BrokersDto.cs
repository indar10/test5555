using Abp.Application.Services.Dto;
using Infogroup.IDMS.Contacts.Dtos;
using System;
using System.Collections.Generic;

namespace Infogroup.IDMS.Brokers.Dtos
{

    
    public class BrokersDto: EntityDto
    {
        public  int DatabaseID { get; set; }       
        public  string cCode { get; set; }        
        public  string cCompany { get; set; }      
        public  string cAddress1 { get; set; }
        public  string cAddress2 { get; set; }        
        public  string cCity { get; set; }
        public  string cState { get; set; }
        public  string cZip { get; set; }
        public  string cPhone { get; set; }
        public  string cFax { get; set; }
        public  string cNotes { get; set; }
        public  bool iIsActive { get; set; }
        public string cAddress { get; set; }
        public  DateTime dCreatedDate { get; set; }
        public  string cCreatedBy { get; set; }
        public  DateTime? dModifiedDate { get; set; }
        public  string cModifiedBy { get; set; }
        public int ContactsCount { get; set; }
        public List<CreateOrEditContactDto> ContactsList { get; set; }


    }
}
