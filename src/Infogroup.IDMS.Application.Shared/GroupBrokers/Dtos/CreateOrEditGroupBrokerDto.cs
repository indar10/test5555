
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.GroupBrokers.Dtos
{
    public class CreateOrEditGroupBrokerDto : EntityDto<int?>
    {

		 public int GroupID { get; set; }
		 
		 
    }
}