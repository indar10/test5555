
using System;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.CampaignAttachments.Dtos
{
    public class CampaignAttachmentDto : EntityDto
    {

		 public string OrderId { get; set; }

        public string FormType { get; set; }

        public string Code { get; set; }

        public int ID { get; set; }

        public string cFileName { get; set; }

        public string RealFileName { get; set; }
        public bool IsDisabled { get; set; }

        public ActionType Action;


    }
}