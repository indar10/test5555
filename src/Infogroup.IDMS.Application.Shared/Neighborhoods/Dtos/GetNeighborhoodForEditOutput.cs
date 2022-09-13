using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Neighborhoods.Dtos
{
    public class GetNeighborhoodForEditOutput
    {
		public CreateOrEditNeighborhoodDto Neighborhood { get; set; }


    }
}