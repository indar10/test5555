using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ListLoadStatuses.Dtos
{
    public class GetListLoadStatusForEditOutput
    {
		public CreateOrEditListLoadStatusDto ListLoadStatus { get; set; }


    }
}