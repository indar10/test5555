using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.Databases.Dtos;
using Infogroup.IDMS.Databases.Exporting;
using Infogroup.IDMS.Divisions;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.UserDatabases;
using Infogroup.IDMS.UserDivisions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Databases
{
    public class DatabasesAppService : IDMSAppServiceBase, IDatabasesAppService
    {
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IRepository<Division, int> _divisionRepository;
        private readonly IRepository<UserDivision, int> _userDivisionRepository;
        private readonly IRepository<Lookup, int> _lookupRepository;
        private readonly IRepository<IDMSUser, int> _userRepository;
        private readonly IRepository<UserDatabase, int> _userDatabaseRepository;
        private readonly IDatabaseExcelExporter _databaseExcelExporter;
        private readonly AppSession _mySession;
        private readonly IRedisIDMSUserCache _userCache;
        private readonly string _databaseNameAscString = "cDatabaseName asc";

        public DatabasesAppService(
            IRepository<Division, int> divisionRepository,
            IDatabaseRepository databaseRepository,
            IRepository<UserDivision, int> userDivisionRepository,
            IRepository<Lookup, int> lookupRepository,
            IRepository<IDMSUser, int> userRepository,
            IRepository<UserDatabase, int> userDatabaseRepository,
            IDatabaseExcelExporter databaseExcelExporter,
            IRedisIDMSUserCache userCache,
            AppSession mySession)
        {
            _databaseRepository = databaseRepository;
            _divisionRepository = divisionRepository;
            _lookupRepository = lookupRepository;
            _userDivisionRepository = userDivisionRepository;
            _userRepository = userRepository;
            _userDatabaseRepository = userDatabaseRepository;
            _databaseExcelExporter = databaseExcelExporter;
            _mySession = mySession;
            _userCache = userCache;
        }

        public async Task<PagedResultDto<DatabaseDto>> GetAllDatabases(GetAllDatabasesInput input)
        {
            try
            {
                input.Filter = input.Filter != null ? input.Filter.Trim() : input.Filter;
                var divisionIds = _userDivisionRepository.GetAll().Where(p => p.UserID == input.UserID).Select(p => p.DivisionID).ToList();
                var filteredDatabases = _databaseRepository.GetAll().Where(p => divisionIds.Contains(p.DivisionId));
                var databases = from o in filteredDatabases
                                join l in _lookupRepository.GetAll() on o.LK_DatabaseType equals l.cCode
                                where l.cLookupValue == DatabaseConsts.DatabaseType
                                join tl in _lookupRepository.GetAll() on o.LK_AccountingDivisionCode equals tl.cCode
                                where tl.cLookupValue == DatabaseConsts.AccountingDivisionCode
                                join user in _userRepository.GetAll() on o.cCreatedBy equals user.cUserID into result
                                from res in result.DefaultIfEmpty()
                                select new DatabaseDto
                                {
                                    LK_DatabaseType = l.cDescription,
                                    cDatabaseName = o.cDatabaseName,                                    
                                    DivisonName=o.Division.cDivisionName,
                                    cAdministratorEmail = o.cAdministratorEmail,
                                    LK_AccountingDivisionCode = tl.cDescription,
                                    Id = o.Id
                                };
                var isDatabaseId = Validation.ValidationHelper.IsNumeric(input.Filter);
                if (!isDatabaseId)
                {
                    databases = databases.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false ||
                    e.LK_DatabaseType.Contains(input.Filter) || e.cDatabaseName.Contains(input.Filter) || e.LK_AccountingDivisionCode.Contains(input.Filter) || e.DivisonName.Contains(input.Filter));
                }
                else
                {
                    var filtersarray = input.Filter.Split(',');
                    databases = databases.Where(p => filtersarray.Contains(p.Id.ToString()));
                }

                var pagedAndFilteredDatabases = databases
                    .OrderBy(input.Sorting ?? _databaseNameAscString)
                    .PageBy(input);

                var totalCount = await databases.CountAsync();
                return new PagedResultDto<DatabaseDto>(
                    totalCount,
                    await pagedAndFilteredDatabases.ToListAsync()
                );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CreateOrEditDatabaseDto> GetDatabaseForEdit(EntityDto input)
        {
            try
            {
                var database = await _databaseRepository.FirstOrDefaultAsync(input.Id);
                var output = ObjectMapper.Map<CreateOrEditDatabaseDto>(database);
                return output;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public  List<DropdownOutputDto> GetDatabaseWithNoAccessCheck()
        {
           try
                {
                    var query = GetDatabaseDataForDropDown();
                    return _databaseRepository.GetDatabaseData(query.Item1, query.Item2).OrderBy(x => x.Label).ToList();
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }


        }

        public async Task CreateOrEdit(CreateOrEditDatabaseDto input)
        {
            try
            {
                input.cDatabaseName = input.cDatabaseName.Trim();
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                ValidateDatabaseName(input);
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var database = ObjectMapper.Map<Database>(input);
                    await _databaseRepository.InsertAsync(database);
                }
                else
                {
                    var updateDatabse = _databaseRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateDatabse);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private void ValidateDatabaseName(CreateOrEditDatabaseDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.cDatabaseName))
            {
                var isExistingDatabaseCount = _databaseRepository.GetAll().Where(p => p.cDatabaseName == input.cDatabaseName)
                    .WhereIf(input.Id > -1, e => Convert.ToInt32(e.Id) != input.Id)
                    .Count();
                if (isExistingDatabaseCount > 0)
                    throw new UserFriendlyException(L("DuplicateDatabaseNameValidation"));
            }
        }

        public GetDatabaseDropDownsDto GetDatabaseDropDownsDto(int userId, int databaseId)
        {
            try
            {
                var divisionIds = _userDivisionRepository.GetAll().Where(p => p.UserID == userId).Select(p => p.DivisionID).ToList();
                return new GetDatabaseDropDownsDto
                {

                    DatabaseTypes = _lookupRepository.GetAll().Where(p => p.iIsActive && p.cLookupValue == DatabaseConsts.DatabaseType)
                                           .Select(dv => new DropdownOutputDto
                                           {
                                               Value = dv.cCode,
                                               Label = dv.cDescription
                                           }).ToList(),
                    DefaultDatabaseType = _lookupRepository.GetAll().FirstOrDefault(p => p.iIsActive && p.cLookupValue == DatabaseConsts.DatabaseType)?.cCode ?? string.Empty,
                    DivisionCodes = _lookupRepository.GetAll().Where(p => p.iIsActive && p.cLookupValue == DatabaseConsts.AccountingDivisionCode)
                                           .Select(dv => new DropdownOutputDto
                                           {
                                               Value = dv.cCode,
                                               Label = dv.cDescription
                                           }).ToList(),
                    DefaultDivisionCode = _databaseRepository.GetAll().FirstOrDefault(p => p.Id == databaseId)?.LK_AccountingDivisionCode ?? string.Empty,
                    Divisions = _userCache.GetDropdownOptions(_mySession.IDMSUserId, UserDropdown.Divisions),
                    DefaultDivision = _databaseRepository.GetAll().FirstOrDefault(p => p.Id == databaseId)?.DivisionId ?? 0,
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Databases_Print)]
        public FileDto ExportDatabaseExcel(int databaseId, string databaseName)
        {
            try
            {
                var databseAccessReport = from UD in _userDatabaseRepository.GetAll()
                                          join D in _databaseRepository.GetAll() on UD.DatabaseId equals D.Id
                                          join td in _divisionRepository.GetAll() on D.DivisionId equals td.Id
                                          join tu in _userRepository.GetAll() on UD.UserId equals tu.Id
                                          where tu.iIsActive && D.Id == databaseId
                                          orderby tu.cFirstName, tu.cLastName
                                          select new GetDatabaseAccessReportDto
                                          {
                                              FirstName = tu.cFirstName,
                                              LastName = tu.cLastName,
                                              UserID = tu.cUserID,
                                              Email = tu.cEmail
                                          };
                databaseName = $"Database Name:{databaseName}";
                return _databaseExcelExporter.ExportToFile(databseAccessReport.ToList(), databaseName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public List<DropdownOutputDto> GetDatabasesForUser()
        {
            try
            {
                return _userCache.GetDropdownOptions(_mySession.IDMSUserId, UserDropdown.Databases);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetExportLayoutDatabasesForUser()
        {
            try
            {
                return _userCache.GetDropdownOptions(_mySession.IDMSUserId, UserDropdown.DatabaseAccess);                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public GetAllDatabaseForUserDto GetAllDatabasesForUser(int userid, int currentDatabaseId)
        {
            try
            {
                var defaultSelection = currentDatabaseId > 0 ? currentDatabaseId : _userCache.GetDefaultDatabaseForCampaign(userid, Convert.ToInt32(_mySession.UserId));

                var dropdownValues = _userCache.GetDropdownOptions(_mySession.IDMSUserId, UserDropdown.Databases);
                return new GetAllDatabaseForUserDto
                {
                    DefaultDatabase = defaultSelection,
                    Databases = dropdownValues
                };

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        private Tuple<string, List<SqlParameter>> GetDatabaseDataForDropDown()

        {
            var query = new Common.QueryBuilder();
            query.AddSelect("l.cDatabaseName,l.ID");
            query.AddFrom("tblDatabase", "l");
            query.AddDistinct();
            (string sql, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);


        }
    }
}