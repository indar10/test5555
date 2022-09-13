
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserAccessObjects.Dtos
{
    public class CreateOrEditUserAccessObjectDto : EntityDto<int?>
    {

		 public int IDMSUserId { get; set; }
		 
		 		 public int AccessObjectId { get; set; }
		 
		 
    }
}