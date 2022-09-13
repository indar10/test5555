
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ListLoadStatuses.Dtos
{
    public class CreateOrEditListLoadStatusDto : EntityDto<int?>
    {

		[Required]
		public string cCalculation { get; set; }
		
		

    }
}