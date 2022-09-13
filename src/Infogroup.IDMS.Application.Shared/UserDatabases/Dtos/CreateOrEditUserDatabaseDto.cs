
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserDatabases.Dtos
{
    public class CreateOrEditUserDatabaseDto : EntityDto<int?>
    {

		 public long UserId { get; set; }
		 
		 		 public int DatabaseId { get; set; }
		 
		 
    }
}