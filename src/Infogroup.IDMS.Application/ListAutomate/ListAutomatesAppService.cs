using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ListAutomate.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.Storage;
using Infogroup.IDMS.Sessions;
using System.Data.SqlClient;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.ListAutomate
{
    [AbpAuthorize(AppPermissions.Pages_ListAutomates)]
    public class ListAutomatesAppService : IDMSAppServiceBase, IListAutomatesAppService
    {
        private readonly IRepository<ListAutomates> _iListAutomateRepository;
        private readonly AppSession _mySession;
        private readonly IListAutomateRepository _customListAutomateRepository;


        public ListAutomatesAppService(IRepository<ListAutomates> iListAutomateRepository, AppSession mySession, IListAutomateRepository customListAutomateRepository)
        {
            _iListAutomateRepository = iListAutomateRepository;
            _customListAutomateRepository = customListAutomateRepository;
            _mySession = mySession;

        }

        #region Scheduled Time
        public GetServerDateTime GetServerDateForTime()
        {
            var serverDateTime = new GetServerDateTime
            {
                Date = DateTime.Now.Date.ToShortDateString(),
                Time = DateTime.Now.AddMinutes(1).TimeOfDay.ToString("hh\\:mm")
            };

            return serverDateTime;
        }
        #endregion

        #region Fetch ListAutomate Records
        public PagedResultDto<IListAutomateDto> GetAllListAutomate(GetAllIListAutomatesInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var query = GetAllListAutomateQuery(input);
                var result = _customListAutomateRepository.GetAllListAutomate(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region ListAutomate Bizness
        private static Tuple<string, string, List<SqlParameter>> GetAllListAutomateQuery(GetAllIListAutomatesInput filters)
        {
            string[] filtersarray = null;

            var ListId = Validation.ValidationHelper.IsNumeric(filters.Filter);
            
            if (!string.IsNullOrEmpty(filters.Filter))
            {
                filtersarray = filters.Filter.Split(',');
            }
            var defaultFilter = $@"AND (lk.cDescription LIKE @FilterText OR l.cSystemFileNameReadyToLoad LIKE @FilterText)";
            //var numericFilter = $@"AND (l.ListId IN @ListId OR l.iInterval IN @ListId OR l.BuildId IN @ListId)";

            string frequency = "LISTCONVERSIONFREQUENCY";
            var query = new Common.QueryBuilder();
            query.AddSelect($"l.Id, l.ListId, l.BuildId,lk.cDescription,l.iInterval, l.cScheduleTime,l.LK_ListConversionFrequency,l.cSystemFileNameReadyToLoad, l.iIsActive");
            query.AddFrom("tblListConversionSchedule", "l");
            query.AddJoin("tbllookup", "lk", "LK_ListConversionFrequency", "l", "JOIN", "cCode");            
            query.AddWhere("And", " lk.cLookupValue", "EQUALTO", frequency);

            if (ListId)
                query.AddWhere("AND", "l.ListId", "IN", filtersarray);

            else
            {
                query.AddWhereString(defaultFilter);
            }
            query.AddSort(filters.Sorting ?? "lk.cDescription ASC");
            query.AddSort(filters.Sorting ?? "ListId ASC");
            query.AddSort(filters.Sorting ?? "BuildId ASC");
            query.AddSort(filters.Sorting ?? "iInterval ASC");
            query.AddSort(filters.Sorting ?? "cScheduleTime ASC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");            
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        #endregion

        #region ListAutomate For Edit       
        public async Task<CreateOrEditIListAutomateDto> GetIListAutomateForEdit(EntityDto input)
        {
            try {
                
                var listEditData = await _customListAutomateRepository.FirstOrDefaultAsync(input.Id);
                var listAutomateEditData = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditIListAutomateDto>(listEditData));
                return listAutomateEditData;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Save ListAutomate Entries
        [AbpAuthorize(AppPermissions.Pages_ListAutomates_Create, AppPermissions.Pages_ListAutomates_Edit)]
        public async Task CreateOrEdit(CreateOrEditIListAutomateDto input)
        {
           
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                ValidateListAutomateInterval(input);
                if (input.Id == null)
                {
                    input.cScheduledBy = _mySession.IDMSUserName;
                    var listAutomate = ObjectMapper.Map<ListAutomates>(input);
                    await _iListAutomateRepository.InsertAsync(listAutomate);
                }
                else
                {
                    var updateOwner = _iListAutomateRepository.Get(input.Id.GetValueOrDefault());
                    input.cScheduledBy = _mySession.IDMSUserName;
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

        #region Validate Interval
        private void ValidateListAutomateInterval(CreateOrEditIListAutomateDto input)
        {
            
            if(input.LK_ListConversionFrequency == 'M' && (input.iInterval < 1 || input.iInterval > 30))
            {
                throw new UserFriendlyException(L("ValidateMonthInterval"));
            }else if (input.LK_ListConversionFrequency == 'Y' && (input.iInterval < 1 || input.iInterval > 365))
            {
                throw new UserFriendlyException(L("ValidateYearInterval"));
            }
            else if (input.LK_ListConversionFrequency == 'W' && (input.iInterval < 1 || input.iInterval > 7))
            {
                throw new UserFriendlyException(L("ValidateWeekInterval"));
            }
            else if (input.LK_ListConversionFrequency == 'D' && (input.iInterval < 0 || input.iInterval > 0))
            {
                throw new UserFriendlyException(L("ValidateDailyInterval"));
            }

        }
        #endregion

        #region Get Frequency For Dropdown
        public List<DropdownOutputDto> GetFrequency()            
        {
            var query = GetListConversionFrequency();
            List<DropdownOutputDto> result = _customListAutomateRepository.GetConversionFrequency(query);
            return result;
        }
        
        private static Tuple<string, string, List<SqlParameter>> GetListConversionFrequency() 
        {                      
            string frequency = "LISTCONVERSIONFREQUENCY";
            var query = new Common.QueryBuilder();
            query.AddSelect($"l.cDescription,l.cCode");
            query.AddFrom("tbllookup", "l");
            query.AddWhere("And", " l.cLookupValue", "EQUALTO", frequency);
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);

        }
        #endregion

    }
}