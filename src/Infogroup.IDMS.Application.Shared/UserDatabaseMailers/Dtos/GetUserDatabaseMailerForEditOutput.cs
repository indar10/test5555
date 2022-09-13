using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserDatabaseMailers.Dtos
{
    public class GetUserDatabaseMailerForEditOutput
    {
		public CreateOrEditUserDatabaseMailerDto UserDatabaseMailer { get; set; }

		public string IDMSUsercUserID { get; set;}

		public string DatabasecDatabaseName { get; set;}


    }
}