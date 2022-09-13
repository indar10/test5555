
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserDatabases.Dtos
{
    public class UserDatabaseDto : EntityDto
    {

		 public long UserId { get; set; }

		 		 public int DatabaseId { get; set; }

		 
    }
}