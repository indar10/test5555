using Infogroup.IDMS.ExportLayouts;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ExportLayoutDetails.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.ExportLayoutDetails
{
	[AbpAuthorize(AppPermissions.Pages_ExportLayoutDetails)]
    public class ExportLayoutDetailsAppService : IDMSAppServiceBase, IExportLayoutDetailsAppService
    {
		 private readonly IRepository<ExportLayoutDetail> _exportLayoutDetailRepository;
		 private readonly IRepository<ExportLayout,int> _lookup_exportLayoutRepository;
		 

		  public ExportLayoutDetailsAppService(IRepository<ExportLayoutDetail> exportLayoutDetailRepository , IRepository<ExportLayout, int> lookup_exportLayoutRepository) 
		  {
			_exportLayoutDetailRepository = exportLayoutDetailRepository;
			_lookup_exportLayoutRepository = lookup_exportLayoutRepository;
		
		  }

		 public async Task<PagedResultDto<GetExportLayoutDetailForViewDto>> GetAll(GetAllExportLayoutDetailsInput input)
         {
			
			var filteredExportLayoutDetails = _exportLayoutDetailRepository.GetAll()
						.Include( e => e.ExportLayoutFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cOutputFieldName.Contains(input.Filter) || e.cFieldDescription.Contains(input.Filter) || e.cFieldName.Contains(input.Filter) || e.cCalculation.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter) || e.cTableNamePrefix.Contains(input.Filter) || e.ctabledescription.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ExportLayoutcDescriptionFilter), e => e.ExportLayoutFk != null && e.ExportLayoutFk.cDescription.ToLower() == input.ExportLayoutcDescriptionFilter.ToLower().Trim());

			var pagedAndFilteredExportLayoutDetails = filteredExportLayoutDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var exportLayoutDetails = from o in pagedAndFilteredExportLayoutDetails
                         join o1 in _lookup_exportLayoutRepository.GetAll() on o.ExportLayoutId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetExportLayoutDetailForViewDto() {
							ExportLayoutDetail = new ExportLayoutDetailDto
							{
                                Id = o.Id
							},
                         	ExportLayoutcDescription = s1 == null ? "" : s1.cDescription.ToString()
						};

            var totalCount = await filteredExportLayoutDetails.CountAsync();

            return new PagedResultDto<GetExportLayoutDetailForViewDto>(
                totalCount,
                await exportLayoutDetails.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_ExportLayoutDetails_Edit)]
		 public async Task<GetExportLayoutDetailForEditOutput> GetExportLayoutDetailForEdit(EntityDto input)
         {
            var exportLayoutDetail = await _exportLayoutDetailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetExportLayoutDetailForEditOutput {ExportLayoutDetail = ObjectMapper.Map<CreateOrEditExportLayoutDetailDto>(exportLayoutDetail)};

            var _lookupExportLayout = await _lookup_exportLayoutRepository.FirstOrDefaultAsync((int)output.ExportLayoutDetail.ExportLayoutId);
            output.ExportLayoutcDescription = _lookupExportLayout.cDescription.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditExportLayoutDetailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_ExportLayoutDetails_Create)]
		 protected virtual async Task Create(CreateOrEditExportLayoutDetailDto input)
         {
            var exportLayoutDetail = ObjectMapper.Map<ExportLayoutDetail>(input);

			

            await _exportLayoutDetailRepository.InsertAsync(exportLayoutDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_ExportLayoutDetails_Edit)]
		 protected virtual async Task Update(CreateOrEditExportLayoutDetailDto input)
         {
            var exportLayoutDetail = await _exportLayoutDetailRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, exportLayoutDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_ExportLayoutDetails_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _exportLayoutDetailRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_ExportLayoutDetails)]
         public async Task<PagedResultDto<ExportLayoutDetailExportLayoutLookupTableDto>> GetAllExportLayoutForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_exportLayoutRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cDescription.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var exportLayoutList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<ExportLayoutDetailExportLayoutLookupTableDto>();
			foreach(var exportLayout in exportLayoutList){
				lookupTableDtoList.Add(new ExportLayoutDetailExportLayoutLookupTableDto
				{
					Id = exportLayout.Id,
					DisplayName = exportLayout.cDescription?.ToString()
				});
			}

            return new PagedResultDto<ExportLayoutDetailExportLayoutLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}