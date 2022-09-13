
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Infogroup.IDMS.ModelDetails.Dtos;

namespace Infogroup.IDMS.Models.Dtos
{
    public class CreateOrEditModelDto : EntityDto<int>
    {
        public bool IsCopyModel { get; set; }

        public ModelSummaryDto ModelSummaryData { get; set; }

        public ModelDetailDto ModelDetailData { get; set; }
    }
}