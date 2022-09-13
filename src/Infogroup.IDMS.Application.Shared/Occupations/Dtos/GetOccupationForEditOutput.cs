using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Occupations.Dtos
{
    public class GetOccupationForEditOutput
    {
		public CreateOrEditOccupationDto Occupation { get; set; }


    }
}