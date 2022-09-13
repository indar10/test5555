using Infogroup.IDMS.MasterLoLs;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ListMailerRequesteds.Exporting;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.ListMailerRequesteds
{
	//[AbpAuthorize(AppPermissions.Pages_ListMailerRequesteds)]
    public class ListMailerRequestedsAppService : IDMSAppServiceBase, IListMailerRequestedsAppService
    {
		 private readonly IRepository<ListMailerRequested> _listMailerRequestedRepository;
		 private readonly IListMailerRequestedsExcelExporter _listMailerRequestedsExcelExporter;
		 private readonly IRepository<MasterLoL,int> _lookup_masterLoLRepository;
		 

		  public ListMailerRequestedsAppService(IRepository<ListMailerRequested> listMailerRequestedRepository, IListMailerRequestedsExcelExporter listMailerRequestedsExcelExporter , IRepository<MasterLoL, int> lookup_masterLoLRepository) 
		  {
			_listMailerRequestedRepository = listMailerRequestedRepository;
			_listMailerRequestedsExcelExporter = listMailerRequestedsExcelExporter;
			_lookup_masterLoLRepository = lookup_masterLoLRepository;
		
		  }

		 public async Task<PagedResultDto<GetListMailerRequestedForViewDto>> GetAll(GetAllListMailerRequestedsInput input)
         {
			
			var filteredListMailerRequesteds = _listMailerRequestedRepository.GetAll()
						.Include( e => e.MasterLoLFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
						.WhereIf(input.MinMailerIDFilter != null, e => e.MailerID >= input.MinMailerIDFilter)
						.WhereIf(input.MaxMailerIDFilter != null, e => e.MailerID <= input.MaxMailerIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cCreatedByFilter),  e => e.cCreatedBy.ToLower() == input.cCreatedByFilter.ToLower().Trim())
						.WhereIf(input.MindCreatedDateFilter != null, e => e.dCreatedDate >= input.MindCreatedDateFilter)
						.WhereIf(input.MaxdCreatedDateFilter != null, e => e.dCreatedDate <= input.MaxdCreatedDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cModifiedByFilter),  e => e.cModifiedBy.ToLower() == input.cModifiedByFilter.ToLower().Trim())
						.WhereIf(input.MindModifiedDateFilter != null, e => e.dModifiedDate >= input.MindModifiedDateFilter)
						.WhereIf(input.MaxdModifiedDateFilter != null, e => e.dModifiedDate <= input.MaxdModifiedDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.MasterLoLcListNameFilter), e => e.MasterLoLFk != null && e.MasterLoLFk.cListName.ToLower() == input.MasterLoLcListNameFilter.ToLower().Trim());

			var pagedAndFilteredListMailerRequesteds = filteredListMailerRequesteds
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var listMailerRequesteds = from o in pagedAndFilteredListMailerRequesteds
                         join o1 in _lookup_masterLoLRepository.GetAll() on o.ListID equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetListMailerRequestedForViewDto() {
							ListMailerRequested = new ListMailerRequestedDto
							{
                                MailerID = o.MailerID,
                                cCreatedBy = o.cCreatedBy,
                                dCreatedDate = o.dCreatedDate,
                                cModifiedBy = o.cModifiedBy,
                                dModifiedDate = o.dModifiedDate,
                                Id = o.Id
							},
                         	MasterLoLcListName = s1 == null ? "" : s1.cListName.ToString()
						};

            var totalCount = await filteredListMailerRequesteds.CountAsync();

            return new PagedResultDto<GetListMailerRequestedForViewDto>(
                totalCount,
                await listMailerRequesteds.ToListAsync()
            );
         }
		 
		 public async Task<GetListMailerRequestedForViewDto> GetListMailerRequestedForView(int id)
         {
            var listMailerRequested = await _listMailerRequestedRepository.GetAsync(id);

            var output = new GetListMailerRequestedForViewDto { ListMailerRequested = ObjectMapper.Map<ListMailerRequestedDto>(listMailerRequested) };

		    if (output.ListMailerRequested.ListID != null)
            {
                var _lookupMasterLoL = await _lookup_masterLoLRepository.FirstOrDefaultAsync((int)output.ListMailerRequested.ListID);
                output.MasterLoLcListName = _lookupMasterLoL.cListName.ToString();
            }
			
            return output;
         }
		 
		// [AbpAuthorize(AppPermissions.Pages_ListMailerRequesteds_Edit)]
		 public async Task<GetListMailerRequestedForEditOutput> GetListMailerRequestedForEdit(EntityDto input)
         {
            var listMailerRequested = await _listMailerRequestedRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetListMailerRequestedForEditOutput {ListMailerRequested = ObjectMapper.Map<CreateOrEditListMailerRequestedDto>(listMailerRequested)};

		    if (output.ListMailerRequested.ListID != null)
            {
                var _lookupMasterLoL = await _lookup_masterLoLRepository.FirstOrDefaultAsync((int)output.ListMailerRequested.ListID);
                output.MasterLoLcListName = _lookupMasterLoL.cListName.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditListMailerRequestedDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_ListMailerRequesteds_Create)]
		 private async Task Create(CreateOrEditListMailerRequestedDto input)
         {
            var listMailerRequested = ObjectMapper.Map<ListMailerRequested>(input);

			

            await _listMailerRequestedRepository.InsertAsync(listMailerRequested);
         }

		// [AbpAuthorize(AppPermissions.Pages_ListMailerRequesteds_Edit)]
		 private async Task Update(CreateOrEditListMailerRequestedDto input)
         {
            var listMailerRequested = await _listMailerRequestedRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, listMailerRequested);
         }

		 //[AbpAuthorize(AppPermissions.Pages_ListMailerRequesteds_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _listMailerRequestedRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetListMailerRequestedsToExcel(GetAllListMailerRequestedsForExcelInput input)
         {
			
			var filteredListMailerRequesteds = _listMailerRequestedRepository.GetAll()
						.Include( e => e.MasterLoLFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
						.WhereIf(input.MinMailerIDFilter != null, e => e.MailerID >= input.MinMailerIDFilter)
						.WhereIf(input.MaxMailerIDFilter != null, e => e.MailerID <= input.MaxMailerIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cCreatedByFilter),  e => e.cCreatedBy.ToLower() == input.cCreatedByFilter.ToLower().Trim())
						.WhereIf(input.MindCreatedDateFilter != null, e => e.dCreatedDate >= input.MindCreatedDateFilter)
						.WhereIf(input.MaxdCreatedDateFilter != null, e => e.dCreatedDate <= input.MaxdCreatedDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cModifiedByFilter),  e => e.cModifiedBy.ToLower() == input.cModifiedByFilter.ToLower().Trim())
						.WhereIf(input.MindModifiedDateFilter != null, e => e.dModifiedDate >= input.MindModifiedDateFilter)
						.WhereIf(input.MaxdModifiedDateFilter != null, e => e.dModifiedDate <= input.MaxdModifiedDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.MasterLoLcListNameFilter), e => e.MasterLoLFk != null && e.MasterLoLFk.cListName.ToLower() == input.MasterLoLcListNameFilter.ToLower().Trim());

			var query = (from o in filteredListMailerRequesteds
                         join o1 in _lookup_masterLoLRepository.GetAll() on o.ListID equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetListMailerRequestedForViewDto() { 
							ListMailerRequested = new ListMailerRequestedDto
							{
                                MailerID = o.MailerID,
                                cCreatedBy = o.cCreatedBy,
                                dCreatedDate = o.dCreatedDate,
                                cModifiedBy = o.cModifiedBy,
                                dModifiedDate = o.dModifiedDate,
                                Id = o.Id
							},
                         	MasterLoLcListName = s1 == null ? "" : s1.cListName.ToString()
						 });


            var listMailerRequestedListDtos = await query.ToListAsync();

            return _listMailerRequestedsExcelExporter.ExportToFile(listMailerRequestedListDtos);
         }



		//[AbpAuthorize(AppPermissions.Pages_ListMailerRequesteds)]
         public async Task<PagedResultDto<ListMailerRequestedMasterLoLLookupTableDto>> GetAllMasterLoLForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_masterLoLRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cListName.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var masterLoLList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<ListMailerRequestedMasterLoLLookupTableDto>();
			foreach(var masterLoL in masterLoLList){
				lookupTableDtoList.Add(new ListMailerRequestedMasterLoLLookupTableDto
				{
					Id = masterLoL.Id,
					DisplayName = masterLoL.cListName?.ToString()
				});
			}

            return new PagedResultDto<ListMailerRequestedMasterLoLLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}