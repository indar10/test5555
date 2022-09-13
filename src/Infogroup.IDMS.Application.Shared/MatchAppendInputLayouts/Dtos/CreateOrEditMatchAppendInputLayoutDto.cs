
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.MatchAppendInputLayouts.Dtos
{
    public class CreateOrEditMatchAppendInputLayoutDto : EntityDto<int?>
    {

		 public int MatchAppendId { get; set; }
		 
		 
    }
}