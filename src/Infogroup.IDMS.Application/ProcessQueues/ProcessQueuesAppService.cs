

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ProcessQueues.Exporting;
using Infogroup.IDMS.ProcessQueues.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Shared.Dtos;
using Abp.UI;
using Infogroup.IDMS.Sessions;

namespace Infogroup.IDMS.ProcessQueues
{
	[AbpAuthorize(AppPermissions.Pages_ProcessQueues)]
    public class ProcessQueuesAppService : IDMSAppServiceBase, IProcessQueuesAppService
    {
		 private readonly IRepository<ProcessQueue> _processQueueRepository;
		 private readonly IProcessQueuesExcelExporter _processQueuesExcelExporter;
		 private readonly IProcessQueueRepository _customProcessQueueRepository;
        private readonly IShortSearch _shortSearch;
        private readonly AppSession _mySession;

        public ProcessQueuesAppService(IRepository<ProcessQueue> processQueueRepository,
              IProcessQueuesExcelExporter processQueuesExcelExporter,
              IProcessQueueRepository customProcessQueueRepository,
                IShortSearch shortSearch,
                AppSession mySession
                )
		  {
			_processQueueRepository = processQueueRepository;
			_processQueuesExcelExporter = processQueuesExcelExporter;
            _customProcessQueueRepository = customProcessQueueRepository;
            _shortSearch = shortSearch;
            _mySession = mySession;
          }

		 public PagedResultDto<ProcessQueueDto> GetAll(GetAllForLookupTableInput input)
         {
            input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
            var shortWhere = _shortSearch.GetWhere(PageID.ProcessQueues, input.Filter);
            var query = GetallProcessQueues(input,shortWhere);
            return _customProcessQueueRepository.GetAllProcessQueue(query);
         }
        public PagedResultDto<ProcessQueueDatabaseDtoForView> GetAllDbSet(GetAllForLookupTableInput input,int PQID)
        {
            var query = GetDbSets(input, PQID);
            return _customProcessQueueRepository.GetAllDbData(query);
        }
        public List<DropdownOutputDto> GetAllLookupsForDropdown()
        {
            try
            {
                var query = GetQueueTypeDropdown();
                return _customProcessQueueRepository.GetLookupData(query.Item1, query.Item2).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        public List<DropdownOutputDto> GetAllLookupsOfProcessTypeForDropdown(string code)
        {
            try
            {
                var query = GetProcessTypeDropdown(code);
                return _customProcessQueueRepository.GetLookupDataForProcess(query.Item1, query.Item2).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public async Task<GetProcessQueueForViewDto> GetProcessQueueForView(int id)
         {
            var processQueue = await _processQueueRepository.GetAsync(id);

            var output = new GetProcessQueueForViewDto { ProcessQueue = ObjectMapper.Map<ProcessQueueDto>(processQueue) };
			
            return output;
         }
		

		 public async Task CreateOrEdit(CreateOrEditProcessQueueDto input)
         {
            try
            {
                if (input.Id == null)
                {
                    await Create(input);
                }
                else
                {
                    await Update(input);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
         }

		 [AbpAuthorize(AppPermissions.Pages_ProcessQueues_Create)]
		 private async Task Create(CreateOrEditProcessQueueDto input)
         {
            input.cCreatedBy= _mySession.IDMSUserName;
            input.dCreatedDate = DateTime.Now;
            var processQueue = ObjectMapper.Map<ProcessQueue>(input);

			

            await _processQueueRepository.InsertAsync(processQueue);
         }

		 [AbpAuthorize(AppPermissions.Pages_ProcessQueues_Edit)]
		 private async Task Update(CreateOrEditProcessQueueDto input)
         {
            try
            {
                var processQueue = await _processQueueRepository.GetAsync(input.Id.GetValueOrDefault());
                input.cModifiedBy = _mySession.IDMSUserName;
                input.dModifiedDate = DateTime.Now;
                //var processQueue = await _processQueueRepository.FirstOrDefaultAsync((int)input.Id);
                ObjectMapper.Map(input, processQueue);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

		 [AbpAuthorize(AppPermissions.Pages_ProcessQueues_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _processQueueRepository.DeleteAsync(input.Id);
         } 

		
        private Tuple<string, string, List<SqlParameter>> GetallProcessQueues(GetAllForLookupTableInput filters,string shortWhere)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"PQ.ID,PQ.cQueueName,PQ.cDescription,PQ.iAllowedThreadCount,PQ.iIsSuspended, " +
                              "PQ.LK_QueueType, PQ.LK_ProcessType, PQ.dCreatedDate, PQ.cCreatedBy, PQ.dModifiedDate," +
                              "PQ.cModifiedBy,l.cDescription AS QueueDescription,lP.cDescription AS ProcessDescription");
            query.AddFrom("tblProcessQueue", "PQ");
            query.AddJoin("tblProcessQueueDatabase", "PQD", "ID", "PQ", "LEFT JOIN", "ProcessQueueId");
            query.AddJoin("tblDatabase", "DB", "DatabaseID", "PQD", "LEFT JOIN", "ID");
            query.AddJoin("tblLookup", "l", "LK_QueueType", "PQ", "INNER JOIN", "cCode").And("l.cLookupValue","EQUALTO","'QUEUETYPE'");
            query.AddJoin("tblLookup", "lP", "LK_ProcessType", "PQ", "INNER JOIN", "cCode").And("lP.cLookupValue", "EQUALTO", "'PROCESSTYPE'").And("lP.cField","EQUALTO", "PQ.LK_QueueType");
            if (shortWhere.Length > 0)
            {
                query.AddWhereString($"({shortWhere})");
            }
            query.AddSort("PQ.ID DESC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
           
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();


            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);

        }
        private Tuple<string, string, List<SqlParameter>> GetDbSets(GetAllForLookupTableInput filters,int PQId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"PQD.ID as PQID,D.ID,D.cDatabaseName");
            query.AddFrom("TBLDatabase", "D");
            query.AddJoin("tblProcessQueueDatabase", "PQD", "ID", "D", "INNER JOIN", "DatabaseID");
            query.AddWhere("", "PQD.ProcessQueueId", "EQUALTO", PQId.ToString());
            query.AddSort("D.cDatabaseName ASC");
            if (filters.MaxResultCount > 1)
            {
                query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            }
            //query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCountProcessQueue().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);

        }
        private Tuple<string,List<SqlParameter>> GetQueueTypeDropdown()
        {
            var query = new Common.QueryBuilder();
            query.AddSelect("l.cLookupValue,l.cDescription,l.cCode,l.iOrderBy");
            query.AddFrom("tblLookup", "l");
            query.AddWhere("", "l.cLookupValue", "EQUALTO", "QUEUETYPE");
            query.AddSort("l.iOrderBy ASC");
            query.AddDistinct();
            (string sql, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);

        }
        private Tuple<string, List<SqlParameter>> GetProcessTypeDropdown(string code)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect("l.cLookupValue,l.cDescription,l.cCode,l.iOrderBy");
            query.AddFrom("tblLookup", "l");
            query.AddWhere("","l.cLookupValue", "EQUALTO", "PROCESSTYPE");
            query.AddWhere("AND","l.cField", "EQUALTO", code.ToString());
            query.AddSort("l.iOrderBy ASC");
            query.AddDistinct();
            (string sql, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);

        }

    }
}