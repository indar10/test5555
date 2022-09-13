using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Neighborhoods
{
	[Table("tblNeighborhood")]
    public class Neighborhood : Entity 
    {

		[Required]
		public virtual string cState { get; set; }
		
		[Required]
		public virtual string cNeighborhood { get; set; }

        public virtual string cCity { get; set; }
		

    }
}