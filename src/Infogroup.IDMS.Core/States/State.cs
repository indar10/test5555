using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.States
{
	[Table("tblState")]
    public class State : Entity 
    {
		public virtual string cStateCode { get; set; }
		
		public virtual string cState { get; set; }
		
		public virtual string cCountyCode { get; set; }
		
		public virtual string cCounty { get; set; }
		
		public virtual string cCity { get; set; }
		
		public virtual string cZip { get; set; }
		
		public virtual int DatabaseID { get; set; }
        
    }
}