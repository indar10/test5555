using Abp.Authorization;
using Abp.UI;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SubSelects.Dtos;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.SegmentLists;
using Infogroup.IDMS.Sessions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Infogroup.IDMS.SubSelectLists;

namespace Infogroup.IDMS.SubSelects
{
    [AbpAuthorize(AppPermissions.Pages_SubSelects)]
    public class SubSelectsAppService : IDMSAppServiceBase, ISubSelectsAppService
    {
		 private readonly IRepository<SubSelect , int> _subSelectRepository;
         private readonly ISegmentListRepository _listRepository;
         private readonly IOrderStatusManager _orderStatusManager;
         private readonly AppSession _mySession;
        private readonly IRepository<SubSelectList> _subSelectListRepository;


        public SubSelectsAppService(IRepository<SubSelect , int> subSelectRepository, 
            ISegmentListRepository listRepository,
            IOrderStatusManager orderStatusManager,
            IRepository<SubSelectList> subSelectListRepository,
            AppSession mySession) 
		  {
			_subSelectRepository = subSelectRepository;
            _listRepository = listRepository;
            _orderStatusManager = orderStatusManager;
            _subSelectListRepository = subSelectListRepository;
            _mySession = mySession;		
		  }

		 public async Task<List<SubSelectForViewDto>> GetAllSubSelect(int segmentId)
         {
            try
            {
                var filteredSubSelects = _subSelectRepository.GetAll().Where(subSelect => subSelect.SegmentId == segmentId);
                var subSelects = await filteredSubSelects
                    .OrderBy(subSelect => subSelect.Id)
                    .Select(subSelect =>  new SubSelectForViewDto {                          
                        cIncludeExclude = subSelect.cIncludeExclude.Trim() == "I" ? "Sub_Include" : "Sub_Exclude",
                        cCompanyIndividual = subSelect.cCompanyIndividual.Trim() == "C" ? "Sub_Company" : "Sub_Individuals",
                        Id = subSelect.Id
                    })
                    .ToListAsync();
                return subSelects;
            }

            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }            
         }

            public async Task<int> CreateSubSelect(CreateOrEditSubSelectDto input)
            {
            try
            {
                if (input.Id == 0)
                {
                    var subSelect = new SubSelect
                    {
                        SegmentId = input.SegmentId,
                        Id = 0,
                        cCompanyIndividual = input.cCompanyIndividual,
                        cIncludeExclude = input.cIncludeExclude,
                        dCreatedDate = DateTime.Now,
                        cCreatedBy = _mySession.IDMSUserName
                    };
                    var subSelectId = await _subSelectRepository.InsertAndGetIdAsync(subSelect);

                    if (input.SourceIds != null && input.SourceIds.Count > 0)
                        await _listRepository.AddSourcesAsync(subSelectId, string.Join(',', input.SourceIds), _mySession.IDMSUserName, true);
                                        
                    await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                    return subSelectId;
                }
                else
                {
                    var sourceIds = string.Empty;
                    var notToDeleteIds = new List<int>();
                    var sourcesModified = false;

                    // Add sources
                    if (input.SourceIds != null && input.SourceIds.Count > 0)
                    {
                        foreach (var item in input.SourceIds)
                        {
                            var subSelectSource = await _subSelectListRepository.FirstOrDefaultAsync(x => x.MasterLOLID == item && x.SubSelectId == input.Id);
                            if (subSelectSource == null)                                                     
                                sourceIds = string.IsNullOrWhiteSpace(sourceIds) ? $"{item}" : $"{sourceIds}, {item}";                            
                            else                            
                                notToDeleteIds.Add(subSelectSource.Id);                            
                        }                        
                        if (!string.IsNullOrWhiteSpace(sourceIds))
                        {
                            sourcesModified = true;
                            await _listRepository.AddSourcesAsync(input.Id, sourceIds, _mySession.IDMSUserName, true);
                        }
                    }

                    // Delete Sources
                    if (input.deletedSourceIds != null && input.deletedSourceIds.Count > 0)
                    {
                        foreach (var value in input.deletedSourceIds)
                        {
                            if (!notToDeleteIds.Contains(value))
                            {
                                sourcesModified = true;
                                await _subSelectListRepository.DeleteAsync(value);
                            }
                        }
                    }
                    if (sourcesModified)
                        await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);

                    return input.Id;
                }

            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public async void UpdateSubSelect(CreateOrEditSubSelectDto input)
        {
            try
            {
                var updateSubSet = _subSelectRepository.Get(input.Id);
                updateSubSet.cIncludeExclude = input.cIncludeExclude;
                updateSubSet.cCompanyIndividual = input.cCompanyIndividual;
                updateSubSet.cModifiedBy = _mySession.IDMSUserName;
                updateSubSet.dModifiedDate = DateTime.Now;
                await _subSelectRepository.UpdateAsync(updateSubSet);
                await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }            
        }

        public async Task DeleteSubSelect(int campaignId, int subSelectId)
         {
            try
            {
               await _subSelectRepository.DeleteAsync(subSelectId);
               await _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }            
         }      
    }
}