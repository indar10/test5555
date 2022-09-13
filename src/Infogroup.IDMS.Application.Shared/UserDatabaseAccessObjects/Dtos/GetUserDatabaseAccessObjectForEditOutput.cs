using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserDatabaseAccessObjects.Dtos
{
    public class GetUserDatabaseAccessObjectForEditOutput
    {
		public CreateOrEditUserDatabaseAccessObjectDto UserDatabaseAccessObject { get; set; }

		public string IDMSUsercFirstName { get; set;}

		public string AccessObjectcCode { get; set;}

		public string DatabasecDatabaseName { get; set;}


    }
}