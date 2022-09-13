using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.SecurityGroups.Dtos
{
    public class GetSecurityGroupForEditOutput
    {
		public CreateOrEditSecurityGroupDto SecurityGroup { get; set; }


    }
}