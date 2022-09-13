

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignFTPs.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.CampaignFTPs
{
	//[AbpAuthorize(AppPermissions.Pages_CampaignFTPs)]
    public class CampaignFTPsAppService : IDMSAppServiceBase, ICampaignFTPsAppService
    {
		 private readonly IRepository<CampaignFTP> _campaignFTPRepository;
		 

		  public CampaignFTPsAppService(IRepository<CampaignFTP> campaignFTPRepository ) 
		  {
			_campaignFTPRepository = campaignFTPRepository;
			
		  }

		 public async Task<PagedResultDto<GetCampaignFTPForViewDto>> GetAll(GetAllCampaignFTPsInput input)
         {
			
			var filteredCampaignFTPs = _campaignFTPRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cFTPServer.Contains(input.Filter) || e.cUserID.Contains(input.Filter) || e.cPassword.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));

			var pagedAndFilteredCampaignFTPs = filteredCampaignFTPs
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var campaignFTPs = from o in pagedAndFilteredCampaignFTPs
                         select new GetCampaignFTPForViewDto() {
							CampaignFTP = new CampaignFTPDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredCampaignFTPs.CountAsync();

            return new PagedResultDto<GetCampaignFTPForViewDto>(
                totalCount,
                await campaignFTPs.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_CampaignFTPs_Edit)]
		 public async Task<GetCampaignFTPForEditOutput> GetCampaignFTPForEdit(EntityDto input)
         {
            var campaignFTP = await _campaignFTPRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCampaignFTPForEditOutput {CampaignFTP = ObjectMapper.Map<CreateOrEditCampaignFTPDto>(campaignFTP)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCampaignFTPDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignFTPs_Create)]
		 private async Task Create(CreateOrEditCampaignFTPDto input)
         {
            var campaignFTP = ObjectMapper.Map<CampaignFTP>(input);

			

            await _campaignFTPRepository.InsertAsync(campaignFTP);
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignFTPs_Edit)]
		 private async Task Update(CreateOrEditCampaignFTPDto input)
         {
            var campaignFTP = await _campaignFTPRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, campaignFTP);
         }

		 [AbpAuthorize(AppPermissions.Pages_CampaignFTPs_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _campaignFTPRepository.DeleteAsync(input.Id);
         } 
    }
}