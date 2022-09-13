using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserGroups.Dtos
{
    public class GetUserGroupForEditOutput
    {
		public CreateOrEditUserGroupDto UserGroup { get; set; }

		public string TblUsercFirstName { get; set;}


    }
}