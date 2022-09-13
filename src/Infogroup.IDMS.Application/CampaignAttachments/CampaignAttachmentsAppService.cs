using Infogroup.IDMS.Campaigns;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignAttachments.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.CampaignAttachments
{
	[AbpAuthorize(AppPermissions.Pages_CampaignAttachments)]
    public class CampaignAttachmentsAppService : IDMSAppServiceBase, ICampaignAttachmentsAppService
    {
		 private readonly IRepository<CampaignAttachment> _campaignAttachmentRepository;
		 private readonly IRepository<Campaign,int> _lookup_campaignRepository;
		 

		  public CampaignAttachmentsAppService(IRepository<CampaignAttachment> campaignAttachmentRepository , IRepository<Campaign, int> lookup_campaignRepository) 
		  {
			_campaignAttachmentRepository = campaignAttachmentRepository;
			_lookup_campaignRepository = lookup_campaignRepository;
		
		  }

		 public async Task<PagedResultDto<GetCampaignAttachmentForViewDto>> GetAll(GetAllCampaignAttachmentsInput input)
         {
			
			var filteredCampaignAttachments = _campaignAttachmentRepository.GetAll()
						.Include( e => e.OrderFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.LK_AttachmentType.Contains(input.Filter) || e.cFileName.Contains(input.Filter) || e.cRealFileName.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.CampaigncDescriptionFilter), e => e.OrderFk != null && e.OrderFk.cDescription.ToLower() == input.CampaigncDescriptionFilter.ToLower().Trim());

			var pagedAndFilteredCampaignAttachments = filteredCampaignAttachments
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var campaignAttachments = from o in pagedAndFilteredCampaignAttachments
                         join o1 in _lookup_campaignRepository.GetAll() on o.OrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetCampaignAttachmentForViewDto() {
							CampaignAttachment = new CampaignAttachmentDto
							{
                                Id = o.Id
							},
                         	CampaigncDescription = s1 == null ? "" : s1.cDescription.ToString()
						};

            var totalCount = await filteredCampaignAttachments.CountAsync();

            return new PagedResultDto<GetCampaignAttachmentForViewDto>(
                totalCount,
                await campaignAttachments.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_CampaignAttachments_Edit)]
		 public async Task<GetCampaignAttachmentForEditOutput> GetCampaignAttachmentForEdit(EntityDto input)
         {
            var campaignAttachment = await _campaignAttachmentRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCampaignAttachmentForEditOutput {CampaignAttachment = ObjectMapper.Map<CreateOrEditCampaignAttachmentDto>(campaignAttachment)};

		    var _lookupCampaign = await _lookup_campaignRepository.FirstOrDefaultAsync((int)output.CampaignAttachment.OrderId);
            output.CampaigncDescription = _lookupCampaign.cDescription.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCampaignAttachmentDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignAttachments_Create)]
		 protected virtual async Task Create(CreateOrEditCampaignAttachmentDto input)
         {
            var campaignAttachment = ObjectMapper.Map<CampaignAttachment>(input);

			

            await _campaignAttachmentRepository.InsertAsync(campaignAttachment);
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignAttachments_Edit)]
		 protected virtual async Task Update(CreateOrEditCampaignAttachmentDto input)
         {
            var campaignAttachment = await _campaignAttachmentRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, campaignAttachment);
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignAttachments_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _campaignAttachmentRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_CampaignAttachments)]
         public async Task<PagedResultDto<CampaignAttachmentCampaignLookupTableDto>> GetAllCampaignForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_campaignRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cDescription.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var campaignList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<CampaignAttachmentCampaignLookupTableDto>();
			foreach(var campaign in campaignList){
				lookupTableDtoList.Add(new CampaignAttachmentCampaignLookupTableDto
				{
					Id = campaign.Id,
					DisplayName = campaign.cDescription?.ToString()
				});
			}

            return new PagedResultDto<CampaignAttachmentCampaignLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}