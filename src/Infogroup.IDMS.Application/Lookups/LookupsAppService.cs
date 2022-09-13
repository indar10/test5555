using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Databases;
using Abp.UI;
using Infogroup.IDMS.Lookups.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Shared.Dtos;
using Abp.UI;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Common;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Lookups
{
	[AbpAuthorize(AppPermissions.Pages_Lookups)]
	public class LookupsAppService : IDMSAppServiceBase, ILookupsAppService
	{
		private readonly IRepository<Database, int> _databaseRepository;
		private readonly AppSession _mySession;
		private readonly ILookupRepository _customLookupRepository;
		private readonly IShortSearch _shortSearch;

		public LookupsAppService(IRepository<Lookup> lookupRepository,
			  IRepository<Database, int> databaseRepository,
			AppSession mySession,
			ILookupRepository customLookupRepository,
			IShortSearch shortSearch

			   )
		{
			_databaseRepository = databaseRepository;
			_customLookupRepository = customLookupRepository;
			_mySession = mySession;
			_shortSearch = shortSearch;
		}



		#region Fetch Lookups
	
		public PagedResultDto<LookupDto> GetAllLookups(GetAllForLookupTableInput input)
		{
			try
			{
				input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
				var shortWhere = _shortSearch.GetWhere(PageID.Lookups, input.Filter);
				var query = GetAllLookupQuery(input, shortWhere);
				var result = _customLookupRepository.GetAllLookupList(query);
				return result;
			}
			catch (Exception ex)
			{
				throw new UserFriendlyException(ex.Message);
			}
		}

		public CreateOrEditLookupDto GetLookupForEdit(int Id)
		{
			try
			{
				var query = GetLookupsById(Id);
				var editLookupData =  _customLookupRepository.GetLookupById(query);
				return ObjectMapper.Map<CreateOrEditLookupDto>(editLookupData);
			}
			catch (Exception ex)
			{
				throw new UserFriendlyException(ex.Message);
			}
		}
		public List<DropdownOutputDto> GetAllLookupsForDropdown()
		{
			try
			{
				var query = GetLookupDataForDropDown();
				return _customLookupRepository.GetLookupData(query.Item1, query.Item2).OrderBy(x => x.Label).ToList();
			}
			catch (Exception e)
			{
				throw new UserFriendlyException(e.Message);
			}
		}
	
		#endregion

		#region Lookup Bizness
		private static Tuple<string, string, List<SqlParameter>> GetAllLookupQuery(GetAllForLookupTableInput filters, string shortWhere)
		{
			if (shortWhere != null & shortWhere.Length > 0)
				filters.Filter = "";

			if (!string.IsNullOrEmpty(filters.Filter))
				filters.Filter = filters.Filter.Trim();

			string[] filtersarray = null;

			var isId = Validation.ValidationHelper.IsNumeric(filters.Filter);
			if (!string.IsNullOrEmpty(filters.Filter))
			{
				filtersarray = filters.Filter.Split(',');
			}

			var defaultFilter = $@" (l.cLookupValue LIKE @FilterText OR l.cDescription LIKE @FilterText)";

			var query = new Common.QueryBuilder();
			query.AddSelect($"l.ID,l.cLookupValue,l.cCode, l.cDescription,l.iOrderBy,l.cField,l.mField,l.iField,l.iIsActive");
			query.AddFrom("tblLookup", "l");

            if (isId)
                query.AddWhere("", "l.ID", "IN", filtersarray);
            else
                query.AddWhereString(defaultFilter);

				if (shortWhere.Length > 0)
				query.AddWhereString($"AND ({shortWhere})");

			query.AddSort(filters.Sorting ?? "ID DESC");
			query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
			query.AddDistinct();

			(string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
			sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

			var sqlCount = query.BuildCount().Item1;
			return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
		}

		private static Tuple<string, string, List<SqlParameter>> GetLookupsById(int lookupId)
		{
			var query = new Common.QueryBuilder();
			query.AddSelect($"l.ID,l.cLookupValue,l.cCode, l.cDescription,l.iOrderBy,l.cField,l.mField,l.iField,l.iIsActive,l.cCreatedBy,l.dCreatedDate");
			query.AddFrom("tblLookup", "l");
			query.AddWhere("", "l.ID", "EQUALTO", lookupId.ToString());

			(string sqlSelect, List<SqlParameter> sqlParams) = query.Build();

			var sqlCount = query.BuildCount().Item1;
			return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
		}
		private Tuple<string, List<SqlParameter>> GetLookupDataForDropDown()

		{
			var query = new Common.QueryBuilder();
			query.AddSelect("l.cLookupValue");
			query.AddFrom("tblLookup", "l");
			query.AddDistinct();
			(string sql, List<SqlParameter> sqlParams) = query.Build();
			return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);


		}

		#endregion

		#region create Lookups

		[AbpAuthorize(AppPermissions.Pages_Lookups_Create, AppPermissions.Pages_Lookups_Edit)]
		public async Task CreateOrEdit(CreateOrEditLookupDto input)
		{
			try
			{
				input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
				ValidateLookups(input);
				if (input.Id == null)
				{

					input.cCreatedBy = _mySession.IDMSUserName;
					input.dCreatedDate = DateTime.Now;
					var lookup = ObjectMapper.Map<Lookup>(input);
					await _customLookupRepository.InsertAsync(lookup);
				}
				else
				{
					var updateLookup = _customLookupRepository.Get(input.Id.GetValueOrDefault());
					input.cModifiedBy = _mySession.IDMSUserName;
					input.dModifiedDate = DateTime.Now;
					ObjectMapper.Map(input, updateLookup);
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
		private void ValidateLookups(CreateOrEditLookupDto input)
		{

			if (!string.IsNullOrWhiteSpace(input.cCode))
			{
				var isExistingCodeCount = 0;
				isExistingCodeCount = _customLookupRepository.GetAll().Count(p => p.Id == input.Id && p.cCode.Trim() == input.cCode.Trim() && p.Id != input.Id);
				if (isExistingCodeCount > 0) throw new UserFriendlyException(L("ValidateCode"));
			}
			if (!string.IsNullOrWhiteSpace(input.cField))
			{
				var isExistingCompanyCount = 0;
				isExistingCompanyCount = _customLookupRepository.GetAll().Count(p => p.Id == input.Id && p.cField == input.cField && p.Id != input.Id);
				if (isExistingCompanyCount > 0) throw new UserFriendlyException(L("ValidateCompany"));
			}
		}
		#endregion

	}
}