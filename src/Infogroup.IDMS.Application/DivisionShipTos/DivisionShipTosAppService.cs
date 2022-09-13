using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.DivisionShipTos.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.UserDivisions;
using Infogroup.IDMS.Divisions;
using Abp.UI;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Validation;

namespace Infogroup.IDMS.DivisionShipTos
{
    [AbpAuthorize(AppPermissions.Pages_DivisionShipTos)]
    public class DivisionShipTosAppService : IDMSAppServiceBase, IDivisionShipTosAppService
    {
        private readonly IRepository<DivisionShipTo,int> _divisionShipToRepository;
        private readonly IRepository<UserDivision,int> _userDivisionRepository;
        private readonly IRepository<Division,int> _divisionRepository;
        private readonly AppSession _mySession;

        public DivisionShipTosAppService(IRepository<DivisionShipTo,int> divisionShipToRepository,
            IRepository<Division,int> divisionRepository,
            IRepository<UserDivision,int> userDivisionRepository, AppSession mySession)
        {
            _divisionShipToRepository = divisionShipToRepository;
            _divisionRepository = divisionRepository;
            _userDivisionRepository = userDivisionRepository;
            _mySession = mySession;
        }

        #region Fetch

        public async Task<PagedResultDto<GetDivisionShipToForViewDto>> GetAllDivisionalShipTo(GetAllDivisionShipTosInput input)
        {
            try
            {
                var divisionIds = _userDivisionRepository.GetAll().Where(p => p.UserID == _mySession.IDMSUserId).Select(p => p.DivisionID).ToList();
                var isFilterEmpty = string.IsNullOrWhiteSpace(input.Filter);
                input.Filter = !isFilterEmpty ? input.Filter.Trim() : input.Filter;
                var filteredDivisionShipTos = _divisionShipToRepository.GetAll().Where(p => divisionIds.Contains(p.DivisionID) && p.iIsActive==input.iIsActiveFilter);
                if (!ValidationHelper.IsNumeric(input.Filter))
                 {
                        filteredDivisionShipTos = filteredDivisionShipTos.WhereIf(!isFilterEmpty, e => e.cCode.Contains(input.Filter) ||
                                    e.cCompany.Contains(input.Filter) || e.cFirstName.Contains(input.Filter) || e.cLastName.Contains(input.Filter)
                                    || e.cEmail.Contains(input.Filter) || e.DivisionFk.cDivisionName.Contains(input.Filter));
                 }
                 else
                 {
                        var filterIDs = input.Filter.Split(',').Select(Int32.Parse).ToList();
                        filteredDivisionShipTos = filteredDivisionShipTos.Where(p => filterIDs.Contains(p.Id));
                 }
                var shipToswithDivision = from shipTos in filteredDivisionShipTos
                                          join division in _divisionRepository.GetAll() on shipTos.DivisionID equals division.Id
                                          select new GetDivisionShipToForViewDto
                                          {
                                              DivisionName = division.cDivisionName,
                                              cCode = shipTos.cCode.Trim(),
                                              cCompany = shipTos.cCompany,
                                              cFirstName = shipTos.cFirstName,
                                              cLastName = shipTos.cLastName,
                                              cCountry = shipTos.cCountry,
                                              cPhone = shipTos.cPhone,
                                              cEmail = shipTos.cEmail,
                                              Id = shipTos.Id
                                          };

                var pagedAndFilteredDivisionShipTos = shipToswithDivision
                 .OrderBy(input.Sorting ?? " cCompany asc")
                 .Distinct().Skip(input.SkipCount).Take(input.MaxResultCount);

                var totalCount = await filteredDivisionShipTos.CountAsync();
                var divisionShipTos = await pagedAndFilteredDivisionShipTos.ToListAsync();

                return new PagedResultDto<GetDivisionShipToForViewDto>(
                    totalCount,
                   divisionShipTos
                );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task<CreateOrEditDivisionShipToDto> GetDivisionShipToForEdit(EntityDto input)
        {
            try
            {
                var shipTo = await _divisionShipToRepository.FirstOrDefaultAsync(input.Id);
                var editShipToData = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditDivisionShipToDto>(shipTo));
                return editShipToData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
     
        #region Save ShipTo
        [AbpAuthorize(AppPermissions.Pages_DivisionShipTos_Create, AppPermissions.Pages_DivisionShipTos_Edit)]
        public async Task CreateOrEdit(CreateOrEditDivisionShipToDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var owner = ObjectMapper.Map<DivisionShipTo>(input);
                    await _divisionShipToRepository.InsertAsync(owner);
                }
                else
                {
                    var updateOwner = _divisionShipToRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateOwner);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            
        }
        #endregion
    }
}