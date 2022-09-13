
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Infogroup.IDMS.SegmentPrevOrderses.Dtos
{
    public class CreateOrEditSegmentPrevOrdersDto
    {
        public int CampaignID { get; set; }
        public List<SegmentPrevOrdersDto> listOfSegmentOrders { get; set; }
    }
}