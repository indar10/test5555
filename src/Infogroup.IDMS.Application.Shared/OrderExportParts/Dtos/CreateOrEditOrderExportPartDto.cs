
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.OrderExportParts.Dtos
{
    public class CreateOrEditOrderExportPartDto : EntityDto<int?>
    {

		 public int OrderId { get; set; }
		 
		 
    }
}