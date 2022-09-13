using System;
using System.Collections.Generic;
using Abp.Domain.Repositories;
using Infogroup.IDMS.GroupBrokers.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Abp.UI;
using System.Data.SqlClient;
using Infogroup.IDMS.Brokers;
using Infogroup.IDMS.Sessions;

namespace Infogroup.IDMS.GroupBrokers
{
    
    public class GroupBrokersAppService : IDMSAppServiceBase, IGroupBrokersAppService
    {
		private readonly IRepository<GroupBroker> _groupBrokerRepository;
        private readonly IRepository<Broker> _customBrokerRepository;
        private readonly IGroupBrokerRepository _customGroupBrokerRepository;
        private readonly AppSession _mySession;

        public GroupBrokersAppService(
            IRepository<GroupBroker> groupBrokerRepository,
            IRepository<Broker> customBrokerRepository,
            IGroupBrokerRepository customGroupBrokerRepository,
            AppSession mySession)
	    {
		    _groupBrokerRepository = groupBrokerRepository;
            _customGroupBrokerRepository = customGroupBrokerRepository;
            _customBrokerRepository = customBrokerRepository;
            _mySession = mySession;
        }

        #region Fetch Group Broker
        public PagedResultDto<GroupBrokerDto> GetAllGroupBroker(GetAllGroupBrokersInputDto input)
        {
            try
            {
                var query = GetAllGroupBrokerQuery(input);
                return _customGroupBrokerRepository.GetAllGroupBrokersList(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region All Group Broker
        [AbpAuthorize(AppPermissions.Pages_SecurityGroups_Create, AppPermissions.Pages_SecurityGroups_Edit)]
        public PagedResultDto<GroupBrokerDto> GetAllBroker(GetAllBrokersInputDto input)
        {
            try
            {
                var query = GetAllBrokerQuery(input);
                return _customGroupBrokerRepository.GetAllBrokersList(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Save
        public void AddSelectedBroker(AddBrokerForGroupDto input)
        {
            try
            {
                _customGroupBrokerRepository.DeleteBroker(input.GroupID);
                foreach (var item in input.SelectedBroker)
                {
                    if (item.IsSelected)
                    {
                        var groupBroker = new GroupBroker
                        {
                            BrokerID = item.Id,
                            GroupID = input.GroupID,
                            cCreatedBy = _mySession.IDMSUserName,
                            dCreatedDate = DateTime.Now
                        };
                        _groupBrokerRepository.Insert(groupBroker);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Business
        private static Tuple<string, string, List<SqlParameter>> GetAllGroupBrokerQuery(GetAllGroupBrokersInputDto filters)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"B.ID, B.cCode, B.cCompany");
            query.AddFrom("tblBroker", "B");
            query.AddJoin("tblGroupBroker", "GB", "ID", "B", "INNER JOIN", "BrokerID");
            query.AddWhere("", "B.iIsActive", "EQUALTO", "1");
            query.AddWhere("AND", "GB.GroupID", "EQUALTO", filters.GroupId.ToString());

            
            query.AddSort(filters.Sorting ?? "cCompany ASC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Sorting}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }

        private static Tuple<string, string, List<SqlParameter>> GetAllBrokerQuery(GetAllBrokersInputDto filters)
        {
            string[] filtersarray = null;
            var isBrokerId = Validation.ValidationHelper.IsNumeric(filters.Filter);
            if (!string.IsNullOrEmpty(filters.Filter))
            {
                filtersarray = filters.Filter.Split(',');
            }

            var codeandCompanyFilter = $@"AND (B.cCode LIKE @FilterText OR B.cCompany LIKE @FilterText)";

            var query = new Common.QueryBuilder();
            query.AddSelect($"B.ID, B.cCode, B.cCompany, CASE WHEN (SELECT 1 FROM tblGroupBroker gB WHERE groupID = {filters.GroupId} AND GB.BrokerID = b.ID  ) = 1 THEN 1 ELSE 0 END AS CHECKED_STATUS");
            query.AddFrom("tblBroker", "B");
            query.AddWhere("", "B.iIsActive", "EQUALTO", "1");
            query.AddWhere("AND", "B.DatabaseID", "EQUALTO", filters.DatabaseId.ToString());

            if (isBrokerId)
                query.AddWhere("AND", "B.ID", "IN", filtersarray);
            else
            {
                query.AddWhereString(codeandCompanyFilter);
            }
            

            query.AddSort(filters.Sorting ?? "cCompany ASC");
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        #endregion
    }
}