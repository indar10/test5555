using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserDivisions.Dtos
{
    public class GetUserDivisionForEditOutput
    {
		public CreateOrEditUserDivisionDto UserDivision { get; set; }

		public string tblUsercFirstName { get; set;}

		public string DivisioncDivisionName { get; set;}


    }
}