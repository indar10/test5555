using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserAccessObjects.Dtos
{
    public class GetUserAccessObjectForEditOutput
    {
		public CreateOrEditUserAccessObjectDto UserAccessObject { get; set; }

		public string IDMSUsercUserID { get; set;}

		public string AccessObjectcCode { get; set;}


    }
}