using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.LoadProcessStatuses.Dtos
{
    public class GetLoadProcessStatusForEditOutput
    {
		public CreateOrEditLoadProcessStatusDto LoadProcessStatus { get; set; }


    }
}