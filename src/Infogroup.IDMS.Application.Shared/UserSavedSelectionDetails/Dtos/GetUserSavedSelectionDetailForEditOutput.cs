using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserSavedSelectionDetails.Dtos
{
    public class GetUserSavedSelectionDetailForEditOutput
    {
		public CreateOrEditUserSavedSelectionDetailDto UserSavedSelectionDetail { get; set; }

		public string UserSavedSelectioncDescription { get; set;}


    }
}