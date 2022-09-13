
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserSavedSelectionDetails.Dtos
{
    public class CreateOrEditUserSavedSelectionDetailDto : EntityDto<int?>
    {

		 public int UserSavedSelectionId { get; set; }
		 
		 
    }
}