using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.MatchAppendStatuses.Dtos
{
    public class GetMatchAppendStatusForEditOutput
    {
		public CreateOrEditMatchAppendStatusDto MatchAppendStatus { get; set; }


    }
}