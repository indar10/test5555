using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.BatchQueues.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Abp.UI;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.IDMSUsers;

namespace Infogroup.IDMS.BatchQueues
{
	[AbpAuthorize(AppPermissions.Pages_Batchqueue)]
	public class BatchQueuesAppService : IDMSAppServiceBase, IBatchQueuesAppService
    {
		private readonly IRepository<BatchQueue> _batchQueueRepository;
		private readonly IRepository<Database, int> _databaseRepository;
		private readonly AppSession _mySession;
		private readonly IBatchQueueRepository _customBatchQueueRepository;
		private readonly IShortSearch _shortSearch;
		private readonly IRedisIDMSUserCache _userCache;

		public BatchQueuesAppService(IRepository<BatchQueue> batchQueueRepository,
			  IRepository<Database, int> databaseRepository,
			AppSession mySession,
			IRedisIDMSUserCache userCache,
			IBatchQueueRepository customBatchQueueRepository, IShortSearch shortSearch) 
		  {
			_batchQueueRepository = batchQueueRepository;
			_databaseRepository = databaseRepository;
			_customBatchQueueRepository = customBatchQueueRepository;
			_mySession = mySession;
			_userCache = userCache;
			_shortSearch = shortSearch;

		}

        #region Fetch Batch Queues 

        public PagedResultDto<BatchQueueDto> GetAll(GetAllBatchQueuesInput input)
         {
			try
			{
				input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
				var DatabaseIds = _userCache.GetDatabaseIDs(_mySession.IDMSUserId);
				var shortWhere = _shortSearch.GetWhere(PageID.BatchProcessQueue, input.Filter);
				var query = GetAllBatchQueuesQuery(input, shortWhere, DatabaseIds);
				var result = _customBatchQueueRepository.GetAllBatchQueues(query);
				return result;
			}
			catch (Exception ex)
			{
				throw new UserFriendlyException(ex.Message);
			}

		}
		public CreateOrEditBatchQueueDto GetQueuesData(int queueId)
		{
			try
			{
				var query = GetLogsById(queueId);
				var editqueueData = _customBatchQueueRepository.GetQueueById(query);
				return ObjectMapper.Map<CreateOrEditBatchQueueDto>(editqueueData);
			}
			catch (Exception ex)
			{
				throw new UserFriendlyException(ex.Message);
			}
		}
		#endregion

		#region status Upadte

		public void CreateOrEdit(int id)
		{
			try
			{
				var input = GetQueuesData(id);
				if (input.iStatusId == BatchQueueStatus.Waiting.GetHashCode())
				{

					var updateBatchQueue = _customBatchQueueRepository.Get(input.Id.GetValueOrDefault());
					input.Result = "cancelled by user : " + _mySession.IDMSUserName;
					input.dEndDate = DateTime.Now;
					input.iStatusId = BatchQueueStatus.Failed.GetHashCode();
					ObjectMapper.Map(input, updateBatchQueue);
					CurrentUnitOfWork.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				throw new UserFriendlyException(ex.Message);
			}
		}
		#endregion

		#region Batch Queues Bizznes
		private static Tuple<string, string, List<SqlParameter>> GetAllBatchQueuesQuery(GetAllBatchQueuesInput filters, string shortWhere, List<int> DatabaseIds)
		{
			if (shortWhere != null & shortWhere.Length > 0)
				filters.Filter = "";

			if (!string.IsNullOrEmpty(filters.Filter))
				filters.Filter = filters.Filter.Trim();

			string[] filtersarray = null;

			string[] DbIds = DatabaseIds.Select(x => x.ToString()).ToArray();

			var isId = Validation.ValidationHelper.IsNumeric(filters.Filter);
			if (!string.IsNullOrEmpty(filters.Filter))
			{
				filtersarray = filters.Filter.Split(',');
			}

			var defaultFilter = $@" (LKPT.cDescription LIKE @FilterText OR DB.cDatabaseName LIKE @FilterText)";

			var query = new Common.QueryBuilder();
			query.AddSelect($"BQ.QueueId,BQ.iStatusId,BQ.FieldName,BQ.ParmData,BQ.cScheduledBy,BQ.dScheduled,BLD.cDescription AS 'Build',LKPT.ProcessTypeId,LKPT.cDescription AS 'ProcessTypeDescription',BQ.Result,DB.cDatabaseName as 'DatabaseName',LKQS.cDescription AS 'Status',CONVERT(VARCHAR(5), DATEADD(MI, DATEDIFF(MI, bq.dStartDate, bq.dEndDate), 0), 108) AS 'dDuration' ");
			query.AddFrom("tblBatchQueue", "BQ");
			query.AddJoin("tblBuild", "BLD", "BuildId", "BQ", "INNER JOIN", "ID");
			query.AddJoin("tblLookupProcessType", "LKPT", "ProcessTypeId", "BQ","INNER JOIN","ProcessTypeId");
			query.AddJoin("tblDatabase", "DB", "DatabaseID", "BLD", "INNER JOIN", "ID");
			query.AddJoin("tblLookup ", "LKQS", "iStatusId", "BQ", "INNER JOIN", "cCode ").And("LKQS.cLookupValue ", "EQUALTO", "'BATCHPROCESSQUEUE'");
            if (isId)
                query.AddWhere("", "BQ.QueueId", "IN", filtersarray);
            else
                query.AddWhereString(defaultFilter);

            if (shortWhere.Length > 0)
                query.AddWhereString($"AND ({shortWhere})");

            query.AddSort(filters.Sorting ?? "QueueId DESC");
			query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            //query.AddDistinct();

            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
			sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

			var sqlCount = query.BuildCountForBatchQueue().Item1;
			return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
		}
		private static Tuple<string, string, List<SqlParameter>> GetLogsById(int queueId)
		{
			var query = new Common.QueryBuilder();
			query.AddSelect($"BQ.QueueId,BQ.iStatusId,BQ.FieldName,BQ.ParmData,BQ.cScheduledBy,BQ.dScheduled,BLD.cDescription AS 'Build',LKPT.ProcessTypeId,LKPT.cDescription AS 'ProcessTypeDescription',BQ.Result,DB.cDatabaseName,LKQS.cDescription AS 'Status',BQ.dEndDate");
			query.AddFrom("tblBatchQueue", "BQ");
			query.AddJoin("tblBuild", "BLD", "BuildId", "BQ", "INNER JOIN", "ID");
			query.AddJoin("tblLookupProcessType", "LKPT", "ProcessTypeId", "BQ", "INNER JOIN", "ProcessTypeId");
			query.AddJoin("tblDatabase", "DB", "DatabaseID", "BLD", "INNER JOIN", "ID");
			query.AddJoin("tblLookup ", "LKQS", "iStatusId", "BQ", "INNER JOIN", "cCode ").And("LKQS.cLookupValue ", "EQUALTO", "'BATCHPROCESSQUEUE'");
			query.AddWhere("", "BQ.QueueId", "EQUALTO",queueId.ToString());

			(string sqlSelect, List<SqlParameter> sqlParams) = query.Build();

			var sqlCount = query.BuildCountForBatchQueue().Item1;
			return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
		}
		#endregion
	}
}