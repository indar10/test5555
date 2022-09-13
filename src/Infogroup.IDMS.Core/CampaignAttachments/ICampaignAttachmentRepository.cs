using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignAttachments.Dtos;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.ExportLayouts.Dtos;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.CampaignAttachments
{
    public interface ICampaignAttachmentRepository : IRepository<CampaignAttachment, int>
    {
        List<CampaignAttachmentDto> GetAttachmentsByID(int campaignId);
    }
}
