
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MatchAppendStatuses.Dtos
{
    public class MatchAppendStatusDto : EntityDto
    {
        public int MatchAppendID { get; set; }

        public int iStatusID { get; set; }

        public bool iIsCurrent { get; set; }

        public DateTime dCreatedDate { get; set; }

        
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

    }
}