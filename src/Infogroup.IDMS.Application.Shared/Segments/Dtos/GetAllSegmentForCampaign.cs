using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class GetAllSegmentForCampaign
    {
        public PagedResultDto<GetSegmentListForView> PagedSegments;

        public int CurrentStatus;
    }
}
