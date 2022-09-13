
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MatchAppendDatabaseUsers.Dtos
{
    public class MatchAppendDatabaseUserDto : EntityDto
    {
        public int UserID { get; set; }

        public int DatabaseID { get; set; }
        
        public string cCreatedBy { get; set; }

        public DateTime cCreatedDate { get; set; }

        public string cModifiedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

    }
}