using Infogroup.IDMS.Campaigns;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignCASApprovals.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.CampaignCASApprovals
{
	//[AbpAuthorize(AppPermissions.Pages_CampaignCASApprovals)]
    public class CampaignCASApprovalsAppService : IDMSAppServiceBase, ICampaignCASApprovalsAppService
    {
		 private readonly IRepository<CampaignCASApproval> _campaignCASApprovalRepository;
		 private readonly IRepository<Campaign,int> _lookup_campaignRepository;
		 

		  public CampaignCASApprovalsAppService(IRepository<CampaignCASApproval> campaignCASApprovalRepository , IRepository<Campaign, int> lookup_campaignRepository) 
		  {
			_campaignCASApprovalRepository = campaignCASApprovalRepository;
			_lookup_campaignRepository = lookup_campaignRepository;
		
		  }

		 public async Task<PagedResultDto<GetCampaignCASApprovalForViewDto>> GetAll(GetAllCampaignCASApprovalsInput input)
         {
			
			var filteredCampaignCASApprovals = _campaignCASApprovalRepository.GetAll()
						.Include( e => e.OrderFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cStatus.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.CampaigncDatabaseNameFilter), e => e.OrderFk != null && e.OrderFk.cDatabaseName.ToLower() == input.CampaigncDatabaseNameFilter.ToLower().Trim());

			var pagedAndFilteredCampaignCASApprovals = filteredCampaignCASApprovals
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var campaignCASApprovals = from o in pagedAndFilteredCampaignCASApprovals
                         join o1 in _lookup_campaignRepository.GetAll() on o.OrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetCampaignCASApprovalForViewDto() {
							CampaignCASApproval = new CampaignCASApprovalDto
							{
                                MasterLOLID = o.MasterLOLID,
                                cStatus = o.cStatus,
                                nBasePrice = o.nBasePrice,
                                dCreatedDate = o.dCreatedDate,
                                cCreatedBy = o.cCreatedBy,
                                cModifiedBy = o.cModifiedBy,
                                dModifiedDate = o.dModifiedDate,
                                Id = o.Id
							},
                         	CampaigncDatabaseName = s1 == null ? "" : s1.cDatabaseName.ToString()
						};

            var totalCount = await filteredCampaignCASApprovals.CountAsync();

            return new PagedResultDto<GetCampaignCASApprovalForViewDto>(
                totalCount,
                await campaignCASApprovals.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_CampaignCASApprovals_Edit)]
		 public async Task<GetCampaignCASApprovalForEditOutput> GetCampaignCASApprovalForEdit(EntityDto input)
         {
            var campaignCASApproval = await _campaignCASApprovalRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCampaignCASApprovalForEditOutput {CampaignCASApproval = ObjectMapper.Map<CreateOrEditCampaignCASApprovalDto>(campaignCASApproval)};


            var _lookupCampaign = await _lookup_campaignRepository.FirstOrDefaultAsync((int)output.CampaignCASApproval.OrderId);
            output.CampaigncDatabaseName = _lookupCampaign.cDatabaseName.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCampaignCASApprovalDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignCASApprovals_Create)]
		 private async Task Create(CreateOrEditCampaignCASApprovalDto input)
         {
            var campaignCASApproval = ObjectMapper.Map<CampaignCASApproval>(input);

			

            await _campaignCASApprovalRepository.InsertAsync(campaignCASApproval);
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignCASApprovals_Edit)]
		 private async Task Update(CreateOrEditCampaignCASApprovalDto input)
         {
            var campaignCASApproval = await _campaignCASApprovalRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, campaignCASApproval);
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignCASApprovals_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _campaignCASApprovalRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_CampaignCASApprovals)]
         public async Task<PagedResultDto<CampaignCASApprovalCampaignLookupTableDto>> GetAllCampaignForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_campaignRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cDatabaseName.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var campaignList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<CampaignCASApprovalCampaignLookupTableDto>();
			foreach(var campaign in campaignList){
				lookupTableDtoList.Add(new CampaignCASApprovalCampaignLookupTableDto
				{
					Id = campaign.Id,
					DisplayName = campaign.cDatabaseName?.ToString()
				});
			}

            return new PagedResultDto<CampaignCASApprovalCampaignLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}