
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.AutoSuppresses.Dtos
{
    public class CreateOrEditAutoSuppressDto : EntityDto<Guid?>
    {

		 public int? DatabaseId { get; set; }
		 
		 
    }
}