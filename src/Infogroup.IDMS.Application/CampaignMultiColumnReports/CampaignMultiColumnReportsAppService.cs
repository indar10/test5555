using Infogroup.IDMS.Campaigns;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.CampaignMultiColumnReports
{
	[AbpAuthorize(AppPermissions.Pages_CampaignMultiColumnReports)]
    public class CampaignMultiColumnReportsAppService : IDMSAppServiceBase, ICampaignMultiColumnReportsAppService
    {
		 private readonly IRepository<CampaignMultiColumnReport> _campaignMultiColumnReportRepository;
		 private readonly IRepository<Campaign,int> _lookup_campaignRepository;
		 

		  public CampaignMultiColumnReportsAppService(IRepository<CampaignMultiColumnReport> campaignMultiColumnReportRepository , IRepository<Campaign, int> lookup_campaignRepository) 
		  {
			_campaignMultiColumnReportRepository = campaignMultiColumnReportRepository;
			_lookup_campaignRepository = lookup_campaignRepository;
		
		  }

		 public async Task<PagedResultDto<GetCampaignMultiColumnReportForViewDto>> GetAll(GetAllCampaignMultiColumnReportsInput input)
         {
			
			var filteredCampaignMultiColumnReports = _campaignMultiColumnReportRepository.GetAll()
						.Include( e => e.OrderFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cFields.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter) || e.cType.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.CampaigncDescriptionFilter), e => e.OrderFk != null && e.OrderFk.cDescription.ToLower() == input.CampaigncDescriptionFilter.ToLower().Trim());

			var pagedAndFilteredCampaignMultiColumnReports = filteredCampaignMultiColumnReports
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var campaignMultiColumnReports = from o in pagedAndFilteredCampaignMultiColumnReports
                         join o1 in _lookup_campaignRepository.GetAll() on o.OrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetCampaignMultiColumnReportForViewDto() {
							CampaignMultiColumnReport = new CampaignMultiColumnReportDto
							{
                                Id = o.Id
							},
                         	CampaigncDescription = s1 == null ? "" : s1.cDescription.ToString()
						};

            var totalCount = await filteredCampaignMultiColumnReports.CountAsync();

            return new PagedResultDto<GetCampaignMultiColumnReportForViewDto>(
                totalCount,
                await campaignMultiColumnReports.ToListAsync()
            );
         }
		 
	
		 public async Task<GetCampaignMultiColumnReportForEditOutput> GetCampaignMultiColumnReportForEdit(EntityDto input)
         {
            var campaignMultiColumnReport = await _campaignMultiColumnReportRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCampaignMultiColumnReportForEditOutput {CampaignMultiColumnReport = ObjectMapper.Map<CreateOrEditCampaignMultiColumnReportDto>(campaignMultiColumnReport)};

		    
                var _lookupCampaign = await _lookup_campaignRepository.FirstOrDefaultAsync((int)output.CampaignMultiColumnReport.OrderId);
                output.CampaigncDescription = _lookupCampaign.cDescription.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCampaignMultiColumnReportDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }


		 private async Task Create(CreateOrEditCampaignMultiColumnReportDto input)
         {
            var campaignMultiColumnReport = ObjectMapper.Map<CampaignMultiColumnReport>(input);

			

            await _campaignMultiColumnReportRepository.InsertAsync(campaignMultiColumnReport);
         }


		 private async Task Update(CreateOrEditCampaignMultiColumnReportDto input)
         {
            var campaignMultiColumnReport = await _campaignMultiColumnReportRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, campaignMultiColumnReport);
         }


         public async Task Delete(EntityDto input)
         {
            await _campaignMultiColumnReportRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_CampaignMultiColumnReports)]
         public async Task<PagedResultDto<CampaignMultiColumnReportCampaignLookupTableDto>> GetAllCampaignForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_campaignRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cDescription.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var campaignList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<CampaignMultiColumnReportCampaignLookupTableDto>();
			foreach(var campaign in campaignList){
				lookupTableDtoList.Add(new CampaignMultiColumnReportCampaignLookupTableDto
				{
					Id = campaign.Id,
					DisplayName = campaign.cDescription?.ToString()
				});
			}

            return new PagedResultDto<CampaignMultiColumnReportCampaignLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}