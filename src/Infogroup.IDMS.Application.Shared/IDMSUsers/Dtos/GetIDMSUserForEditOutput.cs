using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.IDMSUsers.Dtos
{
    public class GetIDMSUserForEditOutput
    {
		public CreateOrEditIDMSUserDto TblUser { get; set; }


    }
}