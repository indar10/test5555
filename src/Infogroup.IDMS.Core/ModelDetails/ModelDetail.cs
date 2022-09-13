using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ModelDetails
{
	[Table("tblModelDetail")]
    public class ModelDetail : Entity 
    {

		public virtual int ModelID { get; set; }
		
		public virtual int BuildID { get; set; }
		
		[Required]
		public virtual string cSQL_Score { get; set; }
		
		[Required]
		public virtual string cSQL_Deciles { get; set; }
		
		[Required]
		public virtual string cSQL_Preselect { get; set; }
		
		[Required]
		public virtual string cSAS_ScoreFileName { get; set; }
		
		[Required]
		public virtual string cSAS_ScoreRealFileName { get; set; }
		
		public virtual DateTime? dSampleScoredDateLast { get; set; }
		
		[Required]
		public virtual string cSampleScoredByLast { get; set; }
		
		public virtual DateTime? dFinalScoredDateLAst { get; set; }
		
		[Required]
		public virtual string cFinalScoredByLast { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		[Required]
		public virtual string cCompleteSQL { get; set; }
		
		public virtual bool iDoNotGenerateModelSQL { get; set; }
		

    }
}