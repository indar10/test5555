using Infogroup.IDMS.Divisions;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.DivisionMailers.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Infogroup.IDMS.UserDivisions;
using Abp.UI;
using Infogroup.IDMS.DivisionMailers.Exporting;
using System.Collections.Generic;
using Infogroup.IDMS.Sessions;
using System.Data.SqlClient;

namespace Infogroup.IDMS.DivisionMailers
{
    [AbpAuthorize(AppPermissions.Pages_DivisionMailers)]
    public class DivisionMailersAppService : IDMSAppServiceBase, IDivisionMailersAppService
    {
        private readonly IRepository<DivisionMailer, int> _divisionMailerRepository;
        private readonly IRepository<Division, int> _divisionRepository;
        private readonly IDivisionMailerRepository _customDivisionMailerRepository;
        private readonly IRepository<UserDivision, int> _userDivisionRepository;
        private readonly IDivisionMailerExcelExporter _divisionMailerExcelExporter;
        private readonly AppSession _mySession;

        public DivisionMailersAppService(
            IRepository<DivisionMailer, int> divisionMailerRepository,
            IRepository<Division, int> divisionRepository,
            IDivisionMailerRepository customDivisionMailerRepository,
            IRepository<UserDivision, int> userDivisionRepository,
            IDivisionMailerExcelExporter divisionMailerExcelExporter,
            AppSession mySession
             )
        {
            _divisionMailerRepository = divisionMailerRepository;
            _divisionRepository = divisionRepository;
            _customDivisionMailerRepository = customDivisionMailerRepository;
            _userDivisionRepository = userDivisionRepository;
            _divisionMailerExcelExporter = divisionMailerExcelExporter;
            _mySession = mySession;
        }

        #region Get division mailer 
        public PagedResultDto<GetDivisionMailerForViewDto> GetAllDivisionMailerList(GetAllDivisionMailersInput filters)
        {            
            try
            {
                filters.Filter = string.IsNullOrEmpty(filters.Filter) ? string.Empty : filters.Filter;
                var divisionIds = _userDivisionRepository.GetAll().Where(p => p.UserID == _mySession.IDMSUserId).Select(p => p.DivisionID).ToList();
                var query = GetAllDivisionMailersQuery(filters, divisionIds.ToArray());
                return  _customDivisionMailerRepository.GetAllDivisionMailerList(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CreateOrEditDivisionMailerDto> GetDivisionMailerForEdit(int DivisionMailerId)
        {
            try
            {
                var divisionMailer = await _divisionMailerRepository.FirstOrDefaultAsync(DivisionMailerId);
                var editDivisionData = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditDivisionMailerDto>(divisionMailer));
                return editDivisionData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        #endregion

        #region Create or edit division mailer 

        [AbpAuthorize(AppPermissions.Pages_DivisionMailers_Create, AppPermissions.Pages_DivisionMailers_Edit)]
        public async Task CreateOrEditDivisionMailer(CreateOrEditDivisionMailerDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                //ValidateDivisionMailers(input);
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var DivisionMailer = ObjectMapper.Map<DivisionMailer>(input);
                    await _customDivisionMailerRepository.InsertAsync(DivisionMailer);
                }
                else
                {
                    var updateDivisionMailer = _customDivisionMailerRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateDivisionMailer);
                    CurrentUnitOfWork.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region Validation
        private void ValidateDivisionMailers(CreateOrEditDivisionMailerDto input)
        {            
            if (!string.IsNullOrWhiteSpace(input.cCompany))
            {
                var isCompanyExist = _divisionRepository.GetAll().Any(p => input.cCompany.Trim().Contains(p.cDivisionName));
                if (isCompanyExist) throw new UserFriendlyException(L("GenericMailerNameValidation"));
            }
        }
        #endregion

        #region division mailer Excel

        [AbpAuthorize(AppPermissions.Pages_DivisionMailers_Print)]
        public FileDto DivisionMailerExcel(GetAllDivisionMailersInput filters)
        {
            try
            {               
                var DivisionMailerList = GetAllDivisionMailerList(filters);
                var excelData = DivisionMailerList.Items.Select(divisionMailer =>
                {
                    var divisionMailerExcelDto = ObjectMapper.Map<DivisionMailerExportDto>(divisionMailer);
                    return divisionMailerExcelDto;
                }).ToList();

                var fileName = $"{L("Customer")}";
                return _divisionMailerExcelExporter.ExportToFile(excelData, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region DivisionMailers Bizness

        private Tuple<string, string, List<SqlParameter>> GetAllDivisionMailersQuery(GetAllDivisionMailersInput filters, int[] DivisionIds)
        {
            if (!string.IsNullOrEmpty(filters.Filter))
                filters.Filter = filters.Filter.Trim();

            // reges to check if global search value is numeric or alphanumeric
            var isMailerId = Validation.ValidationHelper.IsNumeric(filters.Filter);

            // to get division Id's in string Array
            var dvIds = DivisionIds.Select(x => x.ToString()).ToArray();

            // to split comma sepated Id String into string Array
            string[] filtersarray = null;
            if (!string.IsNullOrEmpty(filters.Filter))
            {
                filtersarray = filters.Filter.Split(',');
            }

            var divisionMailerFilter = $@"AND (M.CCOMPANY LIKE @FilterText OR M.CCODE LIKE @FilterText OR M.CFIRSTNAME LIKE @FilterText OR M.CLASTNAME LIKE @FilterText OR M.CEMAIL LIKE @FilterText OR TD.CDIVISIONNAME  LIKE @FilterText)";
            var query = new Common.QueryBuilder();
            query.AddSelect("M.ID, TD.CDIVISIONNAME, M.CCODE, M.CCOMPANY, M.CFIRSTNAME, M.CLASTNAME, M.cADDR1, M.CADDR2, M.CCITY, M.CSTATE, M.CZIP, M.CCOUNTRY, M.CPHONE, M.CFAX, M.CEMAIL, M.MNOTES, M.IISACTIVE");
            query.AddFrom("tblDivisionMailer", "M");
            query.AddJoin("tblDivision", "TD", "DivisionId", "M", "INNER JOIN", "ID");
            query.AddWhere("AND", "M.DivisionId", "IN", dvIds);
            query.AddWhere("AND", "M.IISACTIVE", "EQUALTO", filters.IsActive);

            if (isMailerId)
            {
                query.AddWhere("AND", "M.ID", "IN", filtersarray);
            }
            else
            {
                query.AddWhereString(divisionMailerFilter);
            }
            query.AddSort(filters.Sorting ?? "M.cCompany ASC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");

            (string sql, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sql.ToString(), sqlCount.ToString(), sqlParams);

        }
        #endregion
    }
}