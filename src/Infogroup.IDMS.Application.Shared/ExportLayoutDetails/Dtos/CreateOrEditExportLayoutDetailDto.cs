
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ExportLayoutDetails.Dtos
{
    public class CreateOrEditExportLayoutDetailDto : EntityDto<int?>
    {

		 public int ExportLayoutId { get; set; }
		 
		 
    }
}