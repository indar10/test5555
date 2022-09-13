
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserDatabaseAccessObjects.Dtos
{
    public class CreateOrEditUserDatabaseAccessObjectDto : EntityDto<int?>
    {

		 public int IDMSUserId { get; set; }
		 
		 		 public int AccessObjectId { get; set; }
		 
		 		 public int DatabaseId { get; set; }
		 
		 
    }
}