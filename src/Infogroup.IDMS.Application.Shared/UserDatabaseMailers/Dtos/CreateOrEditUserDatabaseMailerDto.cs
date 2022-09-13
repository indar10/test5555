
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserDatabaseMailers.Dtos
{
    public class CreateOrEditUserDatabaseMailerDto : EntityDto<int?>
    {

		 public int UserId { get; set; }
		 
		 		 public int DatabaseId { get; set; }
		 
		 
    }
}