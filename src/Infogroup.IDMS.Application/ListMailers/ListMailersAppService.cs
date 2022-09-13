using Infogroup.IDMS.MasterLoLs;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ListMailers.Exporting;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.ListMailers
{
	//[AbpAuthorize(AppPermissions.Pages_ListMailers)]
    public class ListMailersAppService : IDMSAppServiceBase, IListMailersAppService
    {
		 private readonly IRepository<ListMailer> _listMailerRepository;
		 private readonly IListMailersExcelExporter _listMailersExcelExporter;
		 private readonly IRepository<MasterLoL,int> _lookup_masterLoLRepository;
		 

		  public ListMailersAppService(IRepository<ListMailer> listMailerRepository, IListMailersExcelExporter listMailersExcelExporter , IRepository<MasterLoL, int> lookup_masterLoLRepository) 
		  {
			_listMailerRepository = listMailerRepository;
			_listMailersExcelExporter = listMailersExcelExporter;
			_lookup_masterLoLRepository = lookup_masterLoLRepository;
		
		  }

        public async Task<PagedResultDto<GetListMailerForViewDto>> GetAll(GetAllListMailersInput input)
        {

            var filteredListMailers = _listMailerRepository.GetAll()
                        .Include(e => e.MasterLoLFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
                        .WhereIf(input.MinMailerIDFilter != null, e => e.MailerID >= input.MinMailerIDFilter)
                        .WhereIf(input.MaxMailerIDFilter != null, e => e.MailerID <= input.MaxMailerIDFilter)
                        .WhereIf(input.MindCreatedDateFilter != null, e => e.dCreatedDate >= input.MindCreatedDateFilter)
                        .WhereIf(input.MaxdCreatedDateFilter != null, e => e.dCreatedDate <= input.MaxdCreatedDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.cCreatedByFilter), e => e.cCreatedBy.ToLower() == input.cCreatedByFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.cModifiedByFilter), e => e.cModifiedBy.ToLower() == input.cModifiedByFilter.ToLower().Trim())
                        .WhereIf(input.MindModifiedDateFilter != null, e => e.dModifiedDate >= input.MindModifiedDateFilter)
                        .WhereIf(input.MaxdModifiedDateFilter != null, e => e.dModifiedDate <= input.MaxdModifiedDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MasterLoLcListNameFilter), e => e.MasterLoLFk != null && e.MasterLoLFk.cListName.ToLower() == input.MasterLoLcListNameFilter.ToLower().Trim());

            var pagedAndFilteredListMailers = filteredListMailers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var listMailers = from o in pagedAndFilteredListMailers
                              join o1 in _lookup_masterLoLRepository.GetAll() on o.ListID equals o1.Id into j1
                              from s1 in j1.DefaultIfEmpty()

                              select new GetListMailerForViewDto()
                              {
                                  ListMailer = new ListMailerDto
                                  {
                                      MailerID = o.MailerID,
                                      dCreatedDate = o.dCreatedDate,
                                      cCreatedBy = o.cCreatedBy,
                                      cModifiedBy = o.cModifiedBy,
                                      dModifiedDate = o.dModifiedDate,
                                      Id = o.Id
                                  },
                                  MasterLoLcListName = s1 == null ? "" : s1.cListName.ToString()
                              };

            var totalCount = await filteredListMailers.CountAsync();

            return new PagedResultDto<GetListMailerForViewDto>(
                totalCount,
                await listMailers.ToListAsync()
            );
        }

        // public async Task<GetListMailerForViewDto> GetListMailerForView(int id)
        //       {
        //          var listMailer = await _listMailerRepository.GetAsync(id);

        //          var output = new GetListMailerForViewDto { ListMailer = ObjectMapper.Map<ListMailerDto>(listMailer) };

        //    if (output.ListMailer.ListID != null)
        //          {
        //              var _lookupMasterLoL = await _lookup_masterLoLRepository.FirstOrDefaultAsync((int)output.ListMailer.ListID);
        //              output.MasterLoLcListName = _lookupMasterLoL.cListName.ToString();
        //          }

        //          return output;
        //       }

        // [AbpAuthorize(AppPermissions.Pages_ListMailers_Edit)]
        // public async Task<GetListMailerForEditOutput> GetListMailerForEdit(EntityDto input)
        //       {
        //          var listMailer = await _listMailerRepository.FirstOrDefaultAsync(input.Id);

        //    var output = new GetListMailerForEditOutput {ListMailer = ObjectMapper.Map<CreateOrEditListMailerDto>(listMailer)};

        //    if (output.ListMailer.ListID != null)
        //          {
        //              var _lookupMasterLoL = await _lookup_masterLoLRepository.FirstOrDefaultAsync((int)output.ListMailer.ListID);
        //              output.MasterLoLcListName = _lookupMasterLoL.cListName.ToString();
        //          }

        //          return output;
        //       }

        // public async Task CreateOrEdit(CreateOrEditListMailerDto input)
        //       {
        //          if(input.Id == null){
        //		await Create(input);
        //	}
        //	else{
        //		await Update(input);
        //	}
        //       }

        // [AbpAuthorize(AppPermissions.Pages_ListMailers_Create)]
        // private async Task Create(CreateOrEditListMailerDto input)
        //       {
        //          var listMailer = ObjectMapper.Map<ListMailer>(input);



        //          await _listMailerRepository.InsertAsync(listMailer);
        //       }

        // [AbpAuthorize(AppPermissions.Pages_ListMailers_Edit)]
        // private async Task Update(CreateOrEditListMailerDto input)
        //       {
        //          var listMailer = await _listMailerRepository.FirstOrDefaultAsync((int)input.Id);
        //           ObjectMapper.Map(input, listMailer);
        //       }

        // [AbpAuthorize(AppPermissions.Pages_ListMailers_Delete)]
        //       public async Task Delete(EntityDto input)
        //       {
        //          await _listMailerRepository.DeleteAsync(input.Id);
        //       } 

        //public async Task<FileDto> GetListMailersToExcel(GetAllListMailersForExcelInput input)
        //       {

        //	var filteredListMailers = _listMailerRepository.GetAll()
        //				.Include( e => e.ListFk)
        //				.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
        //				.WhereIf(input.MinIDFilter != null, e => e.ID >= input.MinIDFilter)
        //				.WhereIf(input.MaxIDFilter != null, e => e.ID <= input.MaxIDFilter)
        //				.WhereIf(input.MinMailerIDFilter != null, e => e.MailerID >= input.MinMailerIDFilter)
        //				.WhereIf(input.MaxMailerIDFilter != null, e => e.MailerID <= input.MaxMailerIDFilter)
        //				.WhereIf(input.MindCreatedDateFilter != null, e => e.dCreatedDate >= input.MindCreatedDateFilter)
        //				.WhereIf(input.MaxdCreatedDateFilter != null, e => e.dCreatedDate <= input.MaxdCreatedDateFilter)
        //				.WhereIf(!string.IsNullOrWhiteSpace(input.cCreatedByFilter),  e => e.cCreatedBy.ToLower() == input.cCreatedByFilter.ToLower().Trim())
        //				.WhereIf(!string.IsNullOrWhiteSpace(input.cModifiedByFilter),  e => e.cModifiedBy.ToLower() == input.cModifiedByFilter.ToLower().Trim())
        //				.WhereIf(input.MindModifiedDateFilter != null, e => e.dModifiedDate >= input.MindModifiedDateFilter)
        //				.WhereIf(input.MaxdModifiedDateFilter != null, e => e.dModifiedDate <= input.MaxdModifiedDateFilter)
        //				.WhereIf(!string.IsNullOrWhiteSpace(input.MasterLoLcListNameFilter), e => e.ListFk != null && e.ListFk.cListName.ToLower() == input.MasterLoLcListNameFilter.ToLower().Trim());

        //	var query = (from o in filteredListMailers
        //                       join o1 in _lookup_masterLoLRepository.GetAll() on o.ListID equals o1.Id into j1
        //                       from s1 in j1.DefaultIfEmpty()

        //                       select new GetListMailerForViewDto() { 
        //					ListMailer = new ListMailerDto
        //					{
        //                              ID = o.ID,
        //                              MailerID = o.MailerID,
        //                              dCreatedDate = o.dCreatedDate,
        //                              cCreatedBy = o.cCreatedBy,
        //                              cModifiedBy = o.cModifiedBy,
        //                              dModifiedDate = o.dModifiedDate,
        //                              Id = o.Id
        //					},
        //                       	MasterLoLcListName = s1 == null ? "" : s1.cListName.ToString()
        //				 });


        //          var listMailerListDtos = await query.ToListAsync();

        //          return _listMailersExcelExporter.ExportToFile(listMailerListDtos);
        //       }



        //[AbpAuthorize(AppPermissions.Pages_ListMailers)]
        //       public async Task<PagedResultDto<ListMailerMasterLoLLookupTableDto>> GetAllMasterLoLForLookupTable(GetAllForLookupTableInput input)
        //       {
        //           var query = _lookup_masterLoLRepository.GetAll().WhereIf(
        //                  !string.IsNullOrWhiteSpace(input.Filter),
        //                 e=> e.cListName.ToString().Contains(input.Filter)
        //              );

        //          var totalCount = await query.CountAsync();

        //          var masterLoLList = await query
        //              .PageBy(input)
        //              .ToListAsync();

        //	var lookupTableDtoList = new List<ListMailerMasterLoLLookupTableDto>();
        //	foreach(var masterLoL in masterLoLList){
        //		lookupTableDtoList.Add(new ListMailerMasterLoLLookupTableDto
        //		{
        //			Id = masterLoL.Id,
        //			DisplayName = masterLoL.cListName?.ToString()
        //		});
        //	}

        //          return new PagedResultDto<ListMailerMasterLoLLookupTableDto>(
        //              totalCount,
        //              lookupTableDtoList
        //          );
        //       }
    }
}