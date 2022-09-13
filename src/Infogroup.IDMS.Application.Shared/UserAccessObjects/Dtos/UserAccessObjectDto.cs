
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserAccessObjects.Dtos
{
    public class UserAccessObjectDto : EntityDto
    {

		 public int IDMSUserId { get; set; }

		 		 public int AccessObjectId { get; set; }

		 
    }
}