
using System;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class MatchAndAppendOutputLayoutDto : EntityDto
    {
        public int MatchAppendID { get; set; }
        
        public string cTableName { get; set; }
        
        public string cFieldName { get; set; }

        public int iOutputLength { get; set; }
        public string OutputLength { get; set; }
        
        public string cOutputFieldName { get; set; }

        public DateTime dCreatedDate { get; set; }
        
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public ActionType ActionType { get; set; }

        public string cModifiedBy { get; set; }

        public int iOutputLayoutOrder { get; set; }
        public string OutputLayoutOrder { get; set; }


    }
}