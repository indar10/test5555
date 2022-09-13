
using System;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class MatchAndAppendInputLayoutDto : EntityDto
    {

        public int MatchAppendId { get; set; }
        public string cFieldName { get; set; }

        public int iStartIndex { get; set; }

        public int iEndIndex { get; set; }

        public int iDataLength { get; set; }

        public int iImportLayoutOrder { get; set; }

        public string StartIndex { get; set; }

        public string EndIndex { get; set; }

        public string DataLength { get; set; }

        public string ImportLayoutOrder { get; set; }

        public string mappingDescription { get; set; }

        public string cMCMapping { get; set; }

        public DateTime dCreatedDate { get; set; }


        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

        public ActionType actionType { get; set; }
        


    }
}