using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.States.Dtos
{
    public class GetStateForEditOutput
    {
		public CreateOrEditStateDto State { get; set; }


    }
}