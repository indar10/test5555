using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.MatchAppendInputLayouts.Dtos
{
    public class GetMatchAppendInputLayoutForEditOutput
    {
		public CreateOrEditMatchAppendInputLayoutDto MatchAppendInputLayout { get; set; }

		public string MatchAppendcClientName { get; set;}


    }
}