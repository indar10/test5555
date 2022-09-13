
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.AutoSuppresses.Dtos
{
    public class AutoSuppressDto : EntityDto<Guid>
    {

		 public int? DatabaseId { get; set; }

		 
    }
}