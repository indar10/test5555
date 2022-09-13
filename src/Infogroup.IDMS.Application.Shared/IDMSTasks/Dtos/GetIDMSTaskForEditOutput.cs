using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class GetIDMSTaskForEditOutput
    {
		public CreateOrEditIDMSTaskDto IDMSTask { get; set; }


    }
}