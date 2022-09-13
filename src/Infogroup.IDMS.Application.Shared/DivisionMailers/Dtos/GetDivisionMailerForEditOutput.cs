using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.DivisionMailers.Dtos
{
    public class GetDivisionMailerForEditOutput
    {
		public CreateOrEditDivisionMailerDto DivisionMailer { get; set; }

		public string DivisioncDivisionName { get; set;}


    }
}