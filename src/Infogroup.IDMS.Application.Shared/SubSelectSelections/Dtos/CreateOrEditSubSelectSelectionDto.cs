
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.SubSelectSelections.Dtos
{
    public class CreateOrEditSubSelectSelectionDto : EntityDto<int?>
    {

		 public int SubSelectId { get; set; }
		 
		 
    }
}