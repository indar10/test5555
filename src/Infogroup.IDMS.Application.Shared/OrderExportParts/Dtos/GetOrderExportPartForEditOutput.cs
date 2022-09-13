using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.OrderExportParts.Dtos
{
    public class GetOrderExportPartForEditOutput
    {
		public CreateOrEditOrderExportPartDto OrderExportPart { get; set; }

		public string CampaigncDescription { get; set;}


    }
}