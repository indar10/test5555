
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserGroups.Dtos
{
    public class CreateOrEditUserGroupDto : EntityDto<int?>
    {

		 public int UserId { get; set; }
		 
		 
    }
}