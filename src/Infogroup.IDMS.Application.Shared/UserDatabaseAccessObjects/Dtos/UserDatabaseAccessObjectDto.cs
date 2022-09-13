
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserDatabaseAccessObjects.Dtos
{
    public class UserDatabaseAccessObjectDto : EntityDto
    {

		 public int IDMSUserId { get; set; }

		 		 public int AccessObjectId { get; set; }

		 		 public int DatabaseId { get; set; }

		 
    }
}