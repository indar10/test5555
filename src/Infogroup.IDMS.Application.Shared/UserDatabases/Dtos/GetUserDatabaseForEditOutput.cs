using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserDatabases.Dtos
{
    public class GetUserDatabaseForEditOutput
    {
		public CreateOrEditUserDatabaseDto UserDatabase { get; set; }

		public string UserName { get; set;}

		public string DatabasecDatabaseName { get; set;}


    }
}