
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ModelDetails.Dtos
{
    public class ModelDetailDto
    {
        public int ModelID { get; set; }

        public int BuildID { get; set; }
        
        public string cSQL_Score { get; set; }
        
        public string cSQL_Deciles { get; set; }
        
        public string cSQL_Preselect { get; set; }
        
        public string cSAS_ScoreFileName { get; set; }
        
        public string cSAS_ScoreRealFileName { get; set; }

        public DateTime? dSampleScoredDateLast { get; set; }
        
        public string cSampleScoredByLast { get; set; }

        public DateTime? dFinalScoredDateLAst { get; set; }
        
        public string cFinalScoredByLast { get; set; }

        public DateTime dCreatedDate { get; set; }
        
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }
        
        public string cCompleteSQL { get; set; }

        public bool iDoNotGenerateModelSQL { get; set; }
    }
}