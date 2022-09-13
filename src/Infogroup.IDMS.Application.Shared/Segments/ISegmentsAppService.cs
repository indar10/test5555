using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Segments.Dtos;

namespace Infogroup.IDMS.Segments
{
    public interface ISegmentsAppService : IApplicationService
    {

        Task<GetAllSegmentForCampaign> GetAllSegmentList(GetSegmentListInput input);
        GetAllSegmentsDropdownOutputDto GetAllSegmentForDropdown(int campaignId, int databaseId);

        Task<CreateOrEditSegmentOutputDto> CreateOrEdit(CreateOrEditSegmentDto segment, int orderStatus);
        Task<ImportSegmentDTO> ValidateCampaignIDForImportSegment(int iCopyToOrderID, int iCopyFromOrderID);

        Task Delete(EntityDto input);

        Task<int> Copy(CopySegmentDto input);
        GetAllSegmentsDropdownOutputDto GetMaxPerGroups(int campaignId, int segmentId);
    }
}