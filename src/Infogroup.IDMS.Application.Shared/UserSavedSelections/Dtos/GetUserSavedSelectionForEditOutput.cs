using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserSavedSelections.Dtos
{
    public class GetUserSavedSelectionForEditOutput
    {
		public CreateOrEditUserSavedSelectionDto UserSavedSelection { get; set; }

		public string DatabasecDatabaseName { get; set;}


    }
}