
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class CreateOrEditMatchAppendDto : EntityDto<int?>
    {
        public MatchAppendDto MatchAppendDto { get; set; }
        public MatchAndAppendInputLayoutDto MatchAppendInputLayout { get; set; }
        public MatchAndAppendOutputLayoutDto MatchAppendOutputLayout { get; set; }

        public List<string> SelectedFields { get; set; }
        public string SelectedTable { get; set; }

        public List<MatchAndAppendInputLayoutDto> MatchAppendInputLayoutList { get; set; }

        public List<MatchAndAppendOutputLayoutDto> MatchAppendOutputLayoutList { get; set; }

    }
}