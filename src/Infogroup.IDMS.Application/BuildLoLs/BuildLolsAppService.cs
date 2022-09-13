using Infogroup.IDMS.Builds;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.BuildLoLs.Exporting;
using Infogroup.IDMS.BuildLoLs.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.BuildLoLs
{
	//[AbpAuthorize(AppPermissions.Pages_BuildLols)]
    public class BuildLolsAppService : IDMSAppServiceBase, IBuildLolsAppService
    {
		 private readonly IRepository<BuildLol> _buildLolRepository;
		 private readonly IBuildLolsExcelExporter _buildLolsExcelExporter;
		 private readonly IRepository<Build,int> _lookup_buildRepository;
		 

		  public BuildLolsAppService(IRepository<BuildLol> buildLolRepository, IBuildLolsExcelExporter buildLolsExcelExporter , IRepository<Build, int> lookup_buildRepository) 
		  {
			_buildLolRepository = buildLolRepository;
			_buildLolsExcelExporter = buildLolsExcelExporter;
			_lookup_buildRepository = lookup_buildRepository;
		
		  }

		 public async Task<PagedResultDto<GetBuildLolForViewDto>> GetAll(GetAllBuildLolsInput input)
         {
			
			var filteredBuildLols = _buildLolRepository.GetAll()
						.Include( e => e.BuildFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.LK_Action.Contains(input.Filter) || e.LK_ActionMonth1.Contains(input.Filter) || e.LK_ActionMonth2.Contains(input.Filter) || e.LK_ActionNextMonth.Contains(input.Filter) || e.LK_QuantityType.Contains(input.Filter) || e.LK_FileType.Contains(input.Filter) || e.cDecisionReasoning.Contains(input.Filter) || e.cSlugDate.Contains(input.Filter) || e.cBatchDateType.Contains(input.Filter) || e.LK_SlugDateType.Contains(input.Filter) || e.cBatch_LastFROM.Contains(input.Filter) || e.cBatch_LastTO.Contains(input.Filter) || e.cBatch_FROM.Contains(input.Filter) || e.cBatch_TO.Contains(input.Filter) || e.Order_No.Contains(input.Filter) || e.Order_ClientPO.Contains(input.Filter) || e.OrderSelection.Contains(input.Filter) || e.Order_Fields.Contains(input.Filter) || e.Order_Comments.Contains(input.Filter) || e.Order_Notes1.Contains(input.Filter) || e.Order_Notes2.Contains(input.Filter) || e.LK_EmailTemplate.Contains(input.Filter) || e.cNote.Contains(input.Filter) || e.cSourceFilenameReadyToLoad.Contains(input.Filter) || e.cSystemFilenameReadyToLoad.Contains(input.Filter) || e.LK_LoadFileType.Contains(input.Filter) || e.LK_LoadFileRowTerminator.Contains(input.Filter) || e.cOnePassFileName.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter) || e.cSQL.Contains(input.Filter) || e.cSQLDescription.Contains(input.Filter) || e.LK_Encoding.Contains(input.Filter))
						.WhereIf(input.MinMasterLolIDFilter != null, e => e.MasterLolID >= input.MinMasterLolIDFilter)
						.WhereIf(input.MaxMasterLolIDFilter != null, e => e.MasterLolID <= input.MaxMasterLolIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_ActionFilter),  e => e.LK_Action.ToLower() == input.LK_ActionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_ActionMonth1Filter),  e => e.LK_ActionMonth1.ToLower() == input.LK_ActionMonth1Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_ActionMonth2Filter),  e => e.LK_ActionMonth2.ToLower() == input.LK_ActionMonth2Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_ActionNextMonthFilter),  e => e.LK_ActionNextMonth.ToLower() == input.LK_ActionNextMonthFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_QuantityTypeFilter),  e => e.LK_QuantityType.ToLower() == input.LK_QuantityTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_FileTypeFilter),  e => e.LK_FileType.ToLower() == input.LK_FileTypeFilter.ToLower().Trim())
						.WhereIf(input.iSkipFirstRowFilter > -1,  e => Convert.ToInt32(e.iSkipFirstRow) == input.iSkipFirstRowFilter )
						.WhereIf(input.iIsActiveFilter > -1,  e => Convert.ToInt32(e.iIsActive) == input.iIsActiveFilter )
						.WhereIf(input.MiniUsageFilter != null, e => e.iUsage >= input.MiniUsageFilter)
						.WhereIf(input.MaxiUsageFilter != null, e => e.iUsage <= input.MaxiUsageFilter)
						.WhereIf(input.MinnTurnsFilter != null, e => e.nTurns >= input.MinnTurnsFilter)
						.WhereIf(input.MaxnTurnsFilter != null, e => e.nTurns <= input.MaxnTurnsFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cDecisionReasoningFilter),  e => e.cDecisionReasoning.ToLower() == input.cDecisionReasoningFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSlugDateFilter),  e => e.cSlugDate.ToLower() == input.cSlugDateFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatchDateTypeFilter),  e => e.cBatchDateType.ToLower() == input.cBatchDateTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_SlugDateTypeFilter),  e => e.LK_SlugDateType.ToLower() == input.LK_SlugDateTypeFilter.ToLower().Trim())
						.WhereIf(input.MiniQuantityPreviousFilter != null, e => e.iQuantityPrevious >= input.MiniQuantityPreviousFilter)
						.WhereIf(input.MaxiQuantityPreviousFilter != null, e => e.iQuantityPrevious <= input.MaxiQuantityPreviousFilter)
						.WhereIf(input.MiniQuantityRequestedFilter != null, e => e.iQuantityRequested >= input.MiniQuantityRequestedFilter)
						.WhereIf(input.MaxiQuantityRequestedFilter != null, e => e.iQuantityRequested <= input.MaxiQuantityRequestedFilter)
						.WhereIf(input.MiniQuantityReceivedDPFilter != null, e => e.iQuantityReceivedDP >= input.MiniQuantityReceivedDPFilter)
						.WhereIf(input.MaxiQuantityReceivedDPFilter != null, e => e.iQuantityReceivedDP <= input.MaxiQuantityReceivedDPFilter)
						.WhereIf(input.MiniQuantityReceivedFilter != null, e => e.iQuantityReceived >= input.MiniQuantityReceivedFilter)
						.WhereIf(input.MaxiQuantityReceivedFilter != null, e => e.iQuantityReceived <= input.MaxiQuantityReceivedFilter)
						.WhereIf(input.MiniQuantityConvertedFilter != null, e => e.iQuantityConverted >= input.MiniQuantityConvertedFilter)
						.WhereIf(input.MaxiQuantityConvertedFilter != null, e => e.iQuantityConverted <= input.MaxiQuantityConvertedFilter)
						.WhereIf(input.MindDateReceivedFilter != null, e => e.dDateReceived >= input.MindDateReceivedFilter)
						.WhereIf(input.MaxdDateReceivedFilter != null, e => e.dDateReceived <= input.MaxdDateReceivedFilter)
						.WhereIf(input.MiniQuantityTotalFilter != null, e => e.iQuantityTotal >= input.MiniQuantityTotalFilter)
						.WhereIf(input.MaxiQuantityTotalFilter != null, e => e.iQuantityTotal <= input.MaxiQuantityTotalFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatch_LastFROMFilter),  e => e.cBatch_LastFROM.ToLower() == input.cBatch_LastFROMFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatch_LastTOFilter),  e => e.cBatch_LastTO.ToLower() == input.cBatch_LastTOFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatch_FROMFilter),  e => e.cBatch_FROM.ToLower() == input.cBatch_FROMFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatch_TOFilter),  e => e.cBatch_TO.ToLower() == input.cBatch_TOFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_NoFilter),  e => e.Order_No.ToLower() == input.Order_NoFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_ClientPOFilter),  e => e.Order_ClientPO.ToLower() == input.Order_ClientPOFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.OrderSelectionFilter),  e => e.OrderSelection.ToLower() == input.OrderSelectionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_FieldsFilter),  e => e.Order_Fields.ToLower() == input.Order_FieldsFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_CommentsFilter),  e => e.Order_Comments.ToLower() == input.Order_CommentsFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_Notes1Filter),  e => e.Order_Notes1.ToLower() == input.Order_Notes1Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_Notes2Filter),  e => e.Order_Notes2.ToLower() == input.Order_Notes2Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_EmailTemplateFilter),  e => e.LK_EmailTemplate.ToLower() == input.LK_EmailTemplateFilter.ToLower().Trim())
						.WhereIf(input.MinddateOrderSentFilter != null, e => e.ddateOrderSent >= input.MinddateOrderSentFilter)
						.WhereIf(input.MaxddateOrderSentFilter != null, e => e.ddateOrderSent <= input.MaxddateOrderSentFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cNoteFilter),  e => e.cNote.ToLower() == input.cNoteFilter.ToLower().Trim())
						.WhereIf(input.MiniCASApprovalToFilter != null, e => e.iCASApprovalTo >= input.MiniCASApprovalToFilter)
						.WhereIf(input.MaxiCASApprovalToFilter != null, e => e.iCASApprovalTo <= input.MaxiCASApprovalToFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSourceFilenameReadyToLoadFilter),  e => e.cSourceFilenameReadyToLoad.ToLower() == input.cSourceFilenameReadyToLoadFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSystemFilenameReadyToLoadFilter),  e => e.cSystemFilenameReadyToLoad.ToLower() == input.cSystemFilenameReadyToLoadFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_LoadFileTypeFilter),  e => e.LK_LoadFileType.ToLower() == input.LK_LoadFileTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_LoadFileRowTerminatorFilter),  e => e.LK_LoadFileRowTerminator.ToLower() == input.LK_LoadFileRowTerminatorFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cOnePassFileNameFilter),  e => e.cOnePassFileName.ToLower() == input.cOnePassFileNameFilter.ToLower().Trim())
						.WhereIf(input.MindCreatedDateFilter != null, e => e.dCreatedDate >= input.MindCreatedDateFilter)
						.WhereIf(input.MaxdCreatedDateFilter != null, e => e.dCreatedDate <= input.MaxdCreatedDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cCreatedByFilter),  e => e.cCreatedBy.ToLower() == input.cCreatedByFilter.ToLower().Trim())
						.WhereIf(input.MindModifiedDateFilter != null, e => e.dModifiedDate >= input.MindModifiedDateFilter)
						.WhereIf(input.MaxdModifiedDateFilter != null, e => e.dModifiedDate <= input.MaxdModifiedDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cModifiedByFilter),  e => e.cModifiedBy.ToLower() == input.cModifiedByFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSQLFilter),  e => e.cSQL.ToLower() == input.cSQLFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSQLDescriptionFilter),  e => e.cSQLDescription.ToLower() == input.cSQLDescriptionFilter.ToLower().Trim())
						.WhereIf(input.MiniLoadQtyFilter != null, e => e.iLoadQty >= input.MiniLoadQtyFilter)
						.WhereIf(input.MaxiLoadQtyFilter != null, e => e.iLoadQty <= input.MaxiLoadQtyFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_EncodingFilter),  e => e.LK_Encoding.ToLower() == input.LK_EncodingFilter.ToLower().Trim())
						.WhereIf(input.iIsMultilineFilter > -1,  e => Convert.ToInt32(e.iIsMultiline) == input.iIsMultilineFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.BuildLK_BuildStatusFilter), e => e.BuildFk != null && e.BuildFk.LK_BuildStatus.ToLower() == input.BuildLK_BuildStatusFilter.ToLower().Trim());

			var pagedAndFilteredBuildLols = filteredBuildLols
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var buildLols = from o in pagedAndFilteredBuildLols
                         join o1 in _lookup_buildRepository.GetAll() on o.BuildId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetBuildLolForViewDto() {
							BuildLol = new BuildLolDto
							{
                                MasterLolID = o.MasterLolID,
                                LK_Action = o.LK_Action,
                                LK_ActionMonth1 = o.LK_ActionMonth1,
                                LK_ActionMonth2 = o.LK_ActionMonth2,
                                LK_ActionNextMonth = o.LK_ActionNextMonth,
                                LK_QuantityType = o.LK_QuantityType,
                                LK_FileType = o.LK_FileType,
                                iSkipFirstRow = o.iSkipFirstRow,
                                iIsActive = o.iIsActive,
                                iUsage = o.iUsage,
                                nTurns = o.nTurns,
                                cDecisionReasoning = o.cDecisionReasoning,
                                cSlugDate = o.cSlugDate,
                                cBatchDateType = o.cBatchDateType,
                                LK_SlugDateType = o.LK_SlugDateType,
                                iQuantityPrevious = o.iQuantityPrevious,
                                iQuantityRequested = o.iQuantityRequested,
                                iQuantityReceivedDP = o.iQuantityReceivedDP,
                                iQuantityReceived = o.iQuantityReceived,
                                iQuantityConverted = o.iQuantityConverted,
                                dDateReceived = o.dDateReceived,
                                iQuantityTotal = o.iQuantityTotal,
                                cBatch_LastFROM = o.cBatch_LastFROM,
                                cBatch_LastTO = o.cBatch_LastTO,
                                cBatch_FROM = o.cBatch_FROM,
                                cBatch_TO = o.cBatch_TO,
                                Order_No = o.Order_No,
                                Order_ClientPO = o.Order_ClientPO,
                                OrderSelection = o.OrderSelection,
                                Order_Fields = o.Order_Fields,
                                Order_Comments = o.Order_Comments,
                                Order_Notes1 = o.Order_Notes1,
                                Order_Notes2 = o.Order_Notes2,
                                LK_EmailTemplate = o.LK_EmailTemplate,
                                ddateOrderSent = o.ddateOrderSent,
                                cNote = o.cNote,
                                iCASApprovalTo = o.iCASApprovalTo,
                                cSourceFilenameReadyToLoad = o.cSourceFilenameReadyToLoad,
                                cSystemFilenameReadyToLoad = o.cSystemFilenameReadyToLoad,
                                LK_LoadFileType = o.LK_LoadFileType,
                                LK_LoadFileRowTerminator = o.LK_LoadFileRowTerminator,
                                cOnePassFileName = o.cOnePassFileName,
                                dCreatedDate = o.dCreatedDate,
                                cCreatedBy = o.cCreatedBy,
                                dModifiedDate = o.dModifiedDate,
                                cModifiedBy = o.cModifiedBy,
                                cSQL = o.cSQL,
                                cSQLDescription = o.cSQLDescription,
                                iLoadQty = o.iLoadQty,
                                LK_Encoding = o.LK_Encoding,
                                iIsMultiline = o.iIsMultiline,
                                Id = o.Id
							},
                         	BuildLK_BuildStatus = s1 == null ? "" : s1.LK_BuildStatus.ToString()
						};

            var totalCount = await filteredBuildLols.CountAsync();

            return new PagedResultDto<GetBuildLolForViewDto>(
                totalCount,
                await buildLols.ToListAsync()
            );
         }
		 
		 public async Task<GetBuildLolForViewDto> GetBuildLolForView(int id)
         {
            var buildLol = await _buildLolRepository.GetAsync(id);

            var output = new GetBuildLolForViewDto { BuildLol = ObjectMapper.Map<BuildLolDto>(buildLol) };

		    if (output.BuildLol.BuildId != null)
            {
                var _lookupBuild = await _lookup_buildRepository.FirstOrDefaultAsync((int)output.BuildLol.BuildId);
                output.BuildLK_BuildStatus = _lookupBuild.LK_BuildStatus.ToString();
            }
			
            return output;
         }
		 
		 //[AbpAuthorize(AppPermissions.Pages_BuildLols_Edit)]
		 public async Task<GetBuildLolForEditOutput> GetBuildLolForEdit(EntityDto input)
         {
            var buildLol = await _buildLolRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetBuildLolForEditOutput {BuildLol = ObjectMapper.Map<CreateOrEditBuildLolDto>(buildLol)};

		    if (output.BuildLol.BuildId != null)
            {
                var _lookupBuild = await _lookup_buildRepository.FirstOrDefaultAsync((int)output.BuildLol.BuildId);
                output.BuildLK_BuildStatus = _lookupBuild.LK_BuildStatus.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditBuildLolDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_BuildLols_Create)]
		 private async Task Create(CreateOrEditBuildLolDto input)
         {
            var buildLol = ObjectMapper.Map<BuildLol>(input);

			

            await _buildLolRepository.InsertAsync(buildLol);
         }

		 //[AbpAuthorize(AppPermissions.Pages_BuildLols_Edit)]
		 private async Task Update(CreateOrEditBuildLolDto input)
         {
            var buildLol = await _buildLolRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, buildLol);
         }

		 //[AbpAuthorize(AppPermissions.Pages_BuildLols_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _buildLolRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetBuildLolsToExcel(GetAllBuildLolsForExcelInput input)
         {
			
			var filteredBuildLols = _buildLolRepository.GetAll()
						.Include( e => e.BuildFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.LK_Action.Contains(input.Filter) || e.LK_ActionMonth1.Contains(input.Filter) || e.LK_ActionMonth2.Contains(input.Filter) || e.LK_ActionNextMonth.Contains(input.Filter) || e.LK_QuantityType.Contains(input.Filter) || e.LK_FileType.Contains(input.Filter) || e.cDecisionReasoning.Contains(input.Filter) || e.cSlugDate.Contains(input.Filter) || e.cBatchDateType.Contains(input.Filter) || e.LK_SlugDateType.Contains(input.Filter) || e.cBatch_LastFROM.Contains(input.Filter) || e.cBatch_LastTO.Contains(input.Filter) || e.cBatch_FROM.Contains(input.Filter) || e.cBatch_TO.Contains(input.Filter) || e.Order_No.Contains(input.Filter) || e.Order_ClientPO.Contains(input.Filter) || e.OrderSelection.Contains(input.Filter) || e.Order_Fields.Contains(input.Filter) || e.Order_Comments.Contains(input.Filter) || e.Order_Notes1.Contains(input.Filter) || e.Order_Notes2.Contains(input.Filter) || e.LK_EmailTemplate.Contains(input.Filter) || e.cNote.Contains(input.Filter) || e.cSourceFilenameReadyToLoad.Contains(input.Filter) || e.cSystemFilenameReadyToLoad.Contains(input.Filter) || e.LK_LoadFileType.Contains(input.Filter) || e.LK_LoadFileRowTerminator.Contains(input.Filter) || e.cOnePassFileName.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter) || e.cSQL.Contains(input.Filter) || e.cSQLDescription.Contains(input.Filter) || e.LK_Encoding.Contains(input.Filter))
						.WhereIf(input.MinMasterLolIDFilter != null, e => e.MasterLolID >= input.MinMasterLolIDFilter)
						.WhereIf(input.MaxMasterLolIDFilter != null, e => e.MasterLolID <= input.MaxMasterLolIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_ActionFilter),  e => e.LK_Action.ToLower() == input.LK_ActionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_ActionMonth1Filter),  e => e.LK_ActionMonth1.ToLower() == input.LK_ActionMonth1Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_ActionMonth2Filter),  e => e.LK_ActionMonth2.ToLower() == input.LK_ActionMonth2Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_ActionNextMonthFilter),  e => e.LK_ActionNextMonth.ToLower() == input.LK_ActionNextMonthFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_QuantityTypeFilter),  e => e.LK_QuantityType.ToLower() == input.LK_QuantityTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_FileTypeFilter),  e => e.LK_FileType.ToLower() == input.LK_FileTypeFilter.ToLower().Trim())
						.WhereIf(input.iSkipFirstRowFilter > -1,  e => Convert.ToInt32(e.iSkipFirstRow) == input.iSkipFirstRowFilter )
						.WhereIf(input.iIsActiveFilter > -1,  e => Convert.ToInt32(e.iIsActive) == input.iIsActiveFilter )
						.WhereIf(input.MiniUsageFilter != null, e => e.iUsage >= input.MiniUsageFilter)
						.WhereIf(input.MaxiUsageFilter != null, e => e.iUsage <= input.MaxiUsageFilter)
						.WhereIf(input.MinnTurnsFilter != null, e => e.nTurns >= input.MinnTurnsFilter)
						.WhereIf(input.MaxnTurnsFilter != null, e => e.nTurns <= input.MaxnTurnsFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cDecisionReasoningFilter),  e => e.cDecisionReasoning.ToLower() == input.cDecisionReasoningFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSlugDateFilter),  e => e.cSlugDate.ToLower() == input.cSlugDateFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatchDateTypeFilter),  e => e.cBatchDateType.ToLower() == input.cBatchDateTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_SlugDateTypeFilter),  e => e.LK_SlugDateType.ToLower() == input.LK_SlugDateTypeFilter.ToLower().Trim())
						.WhereIf(input.MiniQuantityPreviousFilter != null, e => e.iQuantityPrevious >= input.MiniQuantityPreviousFilter)
						.WhereIf(input.MaxiQuantityPreviousFilter != null, e => e.iQuantityPrevious <= input.MaxiQuantityPreviousFilter)
						.WhereIf(input.MiniQuantityRequestedFilter != null, e => e.iQuantityRequested >= input.MiniQuantityRequestedFilter)
						.WhereIf(input.MaxiQuantityRequestedFilter != null, e => e.iQuantityRequested <= input.MaxiQuantityRequestedFilter)
						.WhereIf(input.MiniQuantityReceivedDPFilter != null, e => e.iQuantityReceivedDP >= input.MiniQuantityReceivedDPFilter)
						.WhereIf(input.MaxiQuantityReceivedDPFilter != null, e => e.iQuantityReceivedDP <= input.MaxiQuantityReceivedDPFilter)
						.WhereIf(input.MiniQuantityReceivedFilter != null, e => e.iQuantityReceived >= input.MiniQuantityReceivedFilter)
						.WhereIf(input.MaxiQuantityReceivedFilter != null, e => e.iQuantityReceived <= input.MaxiQuantityReceivedFilter)
						.WhereIf(input.MiniQuantityConvertedFilter != null, e => e.iQuantityConverted >= input.MiniQuantityConvertedFilter)
						.WhereIf(input.MaxiQuantityConvertedFilter != null, e => e.iQuantityConverted <= input.MaxiQuantityConvertedFilter)
						.WhereIf(input.MindDateReceivedFilter != null, e => e.dDateReceived >= input.MindDateReceivedFilter)
						.WhereIf(input.MaxdDateReceivedFilter != null, e => e.dDateReceived <= input.MaxdDateReceivedFilter)
						.WhereIf(input.MiniQuantityTotalFilter != null, e => e.iQuantityTotal >= input.MiniQuantityTotalFilter)
						.WhereIf(input.MaxiQuantityTotalFilter != null, e => e.iQuantityTotal <= input.MaxiQuantityTotalFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatch_LastFROMFilter),  e => e.cBatch_LastFROM.ToLower() == input.cBatch_LastFROMFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatch_LastTOFilter),  e => e.cBatch_LastTO.ToLower() == input.cBatch_LastTOFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatch_FROMFilter),  e => e.cBatch_FROM.ToLower() == input.cBatch_FROMFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cBatch_TOFilter),  e => e.cBatch_TO.ToLower() == input.cBatch_TOFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_NoFilter),  e => e.Order_No.ToLower() == input.Order_NoFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_ClientPOFilter),  e => e.Order_ClientPO.ToLower() == input.Order_ClientPOFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.OrderSelectionFilter),  e => e.OrderSelection.ToLower() == input.OrderSelectionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_FieldsFilter),  e => e.Order_Fields.ToLower() == input.Order_FieldsFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_CommentsFilter),  e => e.Order_Comments.ToLower() == input.Order_CommentsFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_Notes1Filter),  e => e.Order_Notes1.ToLower() == input.Order_Notes1Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.Order_Notes2Filter),  e => e.Order_Notes2.ToLower() == input.Order_Notes2Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_EmailTemplateFilter),  e => e.LK_EmailTemplate.ToLower() == input.LK_EmailTemplateFilter.ToLower().Trim())
						.WhereIf(input.MinddateOrderSentFilter != null, e => e.ddateOrderSent >= input.MinddateOrderSentFilter)
						.WhereIf(input.MaxddateOrderSentFilter != null, e => e.ddateOrderSent <= input.MaxddateOrderSentFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cNoteFilter),  e => e.cNote.ToLower() == input.cNoteFilter.ToLower().Trim())
						.WhereIf(input.MiniCASApprovalToFilter != null, e => e.iCASApprovalTo >= input.MiniCASApprovalToFilter)
						.WhereIf(input.MaxiCASApprovalToFilter != null, e => e.iCASApprovalTo <= input.MaxiCASApprovalToFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSourceFilenameReadyToLoadFilter),  e => e.cSourceFilenameReadyToLoad.ToLower() == input.cSourceFilenameReadyToLoadFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSystemFilenameReadyToLoadFilter),  e => e.cSystemFilenameReadyToLoad.ToLower() == input.cSystemFilenameReadyToLoadFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_LoadFileTypeFilter),  e => e.LK_LoadFileType.ToLower() == input.LK_LoadFileTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_LoadFileRowTerminatorFilter),  e => e.LK_LoadFileRowTerminator.ToLower() == input.LK_LoadFileRowTerminatorFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cOnePassFileNameFilter),  e => e.cOnePassFileName.ToLower() == input.cOnePassFileNameFilter.ToLower().Trim())
						.WhereIf(input.MindCreatedDateFilter != null, e => e.dCreatedDate >= input.MindCreatedDateFilter)
						.WhereIf(input.MaxdCreatedDateFilter != null, e => e.dCreatedDate <= input.MaxdCreatedDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cCreatedByFilter),  e => e.cCreatedBy.ToLower() == input.cCreatedByFilter.ToLower().Trim())
						.WhereIf(input.MindModifiedDateFilter != null, e => e.dModifiedDate >= input.MindModifiedDateFilter)
						.WhereIf(input.MaxdModifiedDateFilter != null, e => e.dModifiedDate <= input.MaxdModifiedDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.cModifiedByFilter),  e => e.cModifiedBy.ToLower() == input.cModifiedByFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSQLFilter),  e => e.cSQL.ToLower() == input.cSQLFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.cSQLDescriptionFilter),  e => e.cSQLDescription.ToLower() == input.cSQLDescriptionFilter.ToLower().Trim())
						.WhereIf(input.MiniLoadQtyFilter != null, e => e.iLoadQty >= input.MiniLoadQtyFilter)
						.WhereIf(input.MaxiLoadQtyFilter != null, e => e.iLoadQty <= input.MaxiLoadQtyFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.LK_EncodingFilter),  e => e.LK_Encoding.ToLower() == input.LK_EncodingFilter.ToLower().Trim())
						.WhereIf(input.iIsMultilineFilter > -1,  e => Convert.ToInt32(e.iIsMultiline) == input.iIsMultilineFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.BuildLK_BuildStatusFilter), e => e.BuildFk != null && e.BuildFk.LK_BuildStatus.ToLower() == input.BuildLK_BuildStatusFilter.ToLower().Trim());

			var query = (from o in filteredBuildLols
                         join o1 in _lookup_buildRepository.GetAll() on o.BuildId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetBuildLolForViewDto() { 
							BuildLol = new BuildLolDto
							{
                                MasterLolID = o.MasterLolID,
                                LK_Action = o.LK_Action,
                                LK_ActionMonth1 = o.LK_ActionMonth1,
                                LK_ActionMonth2 = o.LK_ActionMonth2,
                                LK_ActionNextMonth = o.LK_ActionNextMonth,
                                LK_QuantityType = o.LK_QuantityType,
                                LK_FileType = o.LK_FileType,
                                iSkipFirstRow = o.iSkipFirstRow,
                                iIsActive = o.iIsActive,
                                iUsage = o.iUsage,
                                nTurns = o.nTurns,
                                cDecisionReasoning = o.cDecisionReasoning,
                                cSlugDate = o.cSlugDate,
                                cBatchDateType = o.cBatchDateType,
                                LK_SlugDateType = o.LK_SlugDateType,
                                iQuantityPrevious = o.iQuantityPrevious,
                                iQuantityRequested = o.iQuantityRequested,
                                iQuantityReceivedDP = o.iQuantityReceivedDP,
                                iQuantityReceived = o.iQuantityReceived,
                                iQuantityConverted = o.iQuantityConverted,
                                dDateReceived = o.dDateReceived,
                                iQuantityTotal = o.iQuantityTotal,
                                cBatch_LastFROM = o.cBatch_LastFROM,
                                cBatch_LastTO = o.cBatch_LastTO,
                                cBatch_FROM = o.cBatch_FROM,
                                cBatch_TO = o.cBatch_TO,
                                Order_No = o.Order_No,
                                Order_ClientPO = o.Order_ClientPO,
                                OrderSelection = o.OrderSelection,
                                Order_Fields = o.Order_Fields,
                                Order_Comments = o.Order_Comments,
                                Order_Notes1 = o.Order_Notes1,
                                Order_Notes2 = o.Order_Notes2,
                                LK_EmailTemplate = o.LK_EmailTemplate,
                                ddateOrderSent = o.ddateOrderSent,
                                cNote = o.cNote,
                                iCASApprovalTo = o.iCASApprovalTo,
                                cSourceFilenameReadyToLoad = o.cSourceFilenameReadyToLoad,
                                cSystemFilenameReadyToLoad = o.cSystemFilenameReadyToLoad,
                                LK_LoadFileType = o.LK_LoadFileType,
                                LK_LoadFileRowTerminator = o.LK_LoadFileRowTerminator,
                                cOnePassFileName = o.cOnePassFileName,
                                dCreatedDate = o.dCreatedDate,
                                cCreatedBy = o.cCreatedBy,
                                dModifiedDate = o.dModifiedDate,
                                cModifiedBy = o.cModifiedBy,
                                cSQL = o.cSQL,
                                cSQLDescription = o.cSQLDescription,
                                iLoadQty = o.iLoadQty,
                                LK_Encoding = o.LK_Encoding,
                                iIsMultiline = o.iIsMultiline,
                                Id = o.Id
							},
                         	BuildLK_BuildStatus = s1 == null ? "" : s1.LK_BuildStatus.ToString()
						 });


            var buildLolListDtos = await query.ToListAsync();

            return _buildLolsExcelExporter.ExportToFile(buildLolListDtos);
         }



		//[AbpAuthorize(AppPermissions.Pages_BuildLols)]
         public async Task<PagedResultDto<BuildLolBuildLookupTableDto>> GetAllBuildForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_buildRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.LK_BuildStatus.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var buildList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BuildLolBuildLookupTableDto>();
			foreach(var build in buildList){
				lookupTableDtoList.Add(new BuildLolBuildLookupTableDto
				{
					Id = build.Id,
					DisplayName = build.LK_BuildStatus?.ToString()
				});
			}

            return new PagedResultDto<BuildLolBuildLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}