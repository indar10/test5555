
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserDivisions.Dtos
{
    public class CreateOrEditUserDivisionDto : EntityDto<int?>
    {

		 public int UserID { get; set; }
		 
		 		 public int DivisionID { get; set; }
		 
		 
    }
}