using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.ExportLayouts.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using Infogroup.IDMS.Shared.Dtos;
using Abp.UI;

namespace Infogroup.IDMS.ExportLayouts
{
    public class ExportLayoutsRepository : IDMSRepositoryBase<ExportLayout, int>, IExportLayoutsRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public ExportLayoutsRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public async Task<PagedResultDto<GetExportLayoutForViewDto>> GetAllExportLayoutsList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();
            
            try
            {
                var result = new PagedResultDto<GetExportLayoutForViewDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    var exportLayoutDto = new List<GetExportLayoutForViewDto>();
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {

                        while (dataReader.Read())
                        {
                            exportLayoutDto.Add(new GetExportLayoutForViewDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                DatabaseId = Convert.ToInt32(dataReader["DatabaseID"]),
                                GroupID = Convert.ToInt32(dataReader["GroupID"]),
                                cDescription = dataReader["cDescription"].ToString(),
                                cGroupName = dataReader["cgroupname"].ToString(),
                                iHasKeyCode = Convert.ToBoolean(dataReader["iHasKeyCode"]),
                                iHasPhone = Convert.ToBoolean(dataReader["iHasPhone"]),
                                cOutputCase = dataReader["cOutputCaseLabel"].ToString(),
                                cOutputCaseCode = dataReader["cOutputCase"].ToString()
                            });
                        }
                    }
                    result.Items = exportLayoutDto;
                  
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public List<DropdownOutputDto> GetGroupDataByDatabaseAndUserID(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            var groupNames = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        groupNames.Add(new DropdownOutputDto { Label = dataReader["cGroupName"].ToString(), Value = Convert.ToInt32(dataReader["ID"]) });
                    }
                }
            }

            return groupNames;
        }




        public List<ExportLayoutFieldsDto> GetExportLayoutFields(int iExportLOID)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {
                var layoutFields = new List<ExportLayoutFieldsDto>();
                using (var command = _databaseHelper.CreateCommand(@"
                                                SELECT *, 
                                                CASE WHEN cFieldDescription =' ' THEN cCalculation  ELSE cFieldDescription END AS FLDDESCR,
                                                CASE WHEN cFieldDescription =' ' THEN 1  ELSE 0 END AS IsCalculated
                                                FROM tblExportLayoutDetail WHERE ExportLayoutID = " + iExportLOID + " ORDER BY iExportOrder", CommandType.Text))

                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            layoutFields.Add(new ExportLayoutFieldsDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                iExportOrder = Convert.ToInt32(dataReader["iExportOrder"]),
                                cOutputFieldName = dataReader["cOutputFieldName"].ToString(),
                                FLDDESCR = dataReader["FLDDESCR"].ToString(),
                                iWidth = Convert.ToInt32(dataReader["iWidth"])
                            });
                        }
                    }
                }

                return layoutFields;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void CopyExportLayout(GetExportLayoutForViewDto record)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@ExportLayoutID", record.ID));
            sqlParameters.Add(new SqlParameter("@InitiatedBy", record.cCreatedBy));


            using (var command = _databaseHelper.CreateCommand("usp_CopyExportLayout", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }
        }

        public int CopyAllExportLayout(CopyAllExportLayoutDto input, string intiatedBy, string layoutIds)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var sqlParameters = new List<SqlParameter>();
                var ErrorNo = 0;

                sqlParameters.Add(new SqlParameter("@FromGroupID", input.GroupFromId));
                sqlParameters.Add(new SqlParameter("@ToGroupID", input.GroupToId));
                sqlParameters.Add(new SqlParameter("@InitiatedBy", intiatedBy));
                sqlParameters.Add(new SqlParameter("@Layouts", layoutIds));
                sqlParameters.Add(new SqlParameter("@ErrorNo", ErrorNo));

                using (var command = _databaseHelper.CreateCommand("usp_CopyExportLayoutGroupLevel", CommandType.StoredProcedure, sqlParameters.ToArray()))
                {
                    ErrorNo = Convert.ToInt32(command.ExecuteScalar());
                }
                return ErrorNo;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<GetExportLayoutSelectedFieldsDto> GetExportLayoutSelectedFields(int iExportLOID, int buildId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var selectedFields = new List<GetExportLayoutSelectedFieldsDto>();
            using (var command = _databaseHelper.CreateCommand($@"SELECT *, 
                                 CASE WHEN cFieldDescription =' ' THEN cCalculation  ELSE cFieldDescription END AS FLDDESCR,
                                 CASE WHEN cFieldDescription =' ' THEN 1  ELSE 0 END AS IsCalculated,	
	                             BT.ctabledescription+' ('+ BT.cTableName+')' as TableDescription
                                 FROM tblExportLayoutDetail  ELD
                                 left outer join 
                                (select LEFT(cTableName, CHARINDEX('_',cTableName)-1) as cTableName,ctabledescription from tblbuildtable where buildid={buildId}
                                 UNION 						         SELECT  LEFT(cTableName, CHARINDEX('_',cTableName)-1) AS cTableName,ctabledescription						         FROM   tblBuild 							     INNER JOIN tblExternalBuildTableDatabase ExDB 								 ON ExDB.DatabaseID = tblBuild.DatabaseID AND tblBuild.ID ={buildId}							     INNER JOIN  tblBuildTable 								 ON  tblBuildTable.ID = ExDB.BuildTableID) BT
                                on BT.cTableName = CAse when ELD.ctablenamePrefix = 'MainTable' then 'tblMain' else ELD.ctablenamePrefix end
                                WHERE ExportLayoutID = {iExportLOID} ORDER BY iExportOrder
                                ", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        selectedFields.Add(new GetExportLayoutSelectedFieldsDto
                        {
                            ID = Convert.ToInt32(dataReader["ID"]),
                            Order = Convert.ToInt32(dataReader["iExportOrder"]),
                            OutputFieldName = dataReader["cOutputFieldName"].ToString(),
                            Formula = dataReader["cCalculation"].ToString(),
                            Width = Convert.ToInt32(dataReader["iWidth"]),
                            tablePrefix = dataReader["cTableNamePrefix"].ToString(),
                            tableDescription = dataReader["TableDescription"].ToString(),
                            iIsCalculatedField = dataReader["iIsCalculatedField"] is DBNull ? false: Convert.ToBoolean(dataReader["iIsCalculatedField"]),
                            fieldName =dataReader["cFieldName"].ToString(),
                            fieldDescription = dataReader["FLDDESCR"].ToString()
                        });
                    }
                }
            }

            return selectedFields;
        }


        public List<GetExportLayoutAddFieldsDto> GetExportLayoutAddFields(int tableId, int exportLayoutId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var outputlayoutTemplete = new List<GetExportLayoutAddFieldsDto>();


            var commandString =
                  $@"SELECT BTL.cFieldName, BTL.cFieldDescription,CASE WHEN BT.LK_TableType = 'M' THEN 'MainTable' ELSE  Left(BT.cTableName,CHARINDEX('_',BT.cTableName)-1) END + cFieldName AS [FieldTableKey]
                        FROM tblBuildTableLayout BTL with(nolock) 
                        inner join tblBuildTable BT with(nolock) ON BT.ID = BTL.BuildTableID
                        WHERE BTL.iAllowExport = 1 AND BTL.BuildTableID = {tableId} AND CASE WHEN BT.LK_TableType = 'M' THEN 'MainTable' ELSE  Left(BT.cTableName,CHARINDEX('_',BT.cTableName)-1) END
                            + cFieldName  
                                NOT IN (Select cTableNamePrefix+cFieldName FROM tblExportLayoutDetail with(nolock) WHERE ExportLayoutID = {exportLayoutId}) ORDER BY BTL.cFieldDescription";


            using (var command = _databaseHelper.CreateCommand(commandString, CommandType.Text))

            {

                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        outputlayoutTemplete.Add(new GetExportLayoutAddFieldsDto { FieldName = dataReader["cFieldName"].ToString(), FieldDescription = dataReader["cFieldDescription"].ToString(), FieldTableKey = dataReader["FieldTableKey"].ToString() });
                    }
                }
            }
            return outputlayoutTemplete;
        }

        public void ReorderExportLayoutOrderId(int id, int orderId, string modifiedBy)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@MoveFrom", id));
            sqlParameters.Add(new SqlParameter("@MoveTo", orderId));
            sqlParameters.Add(new SqlParameter("@InitiatedBy", modifiedBy));


            using (var command = _databaseHelper.CreateCommand("usp_MoveExportLayoutFields", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }
        }

        public PagedResultDto<GetCopyAllExportLayoutForViewDto> GetExportLayoutForCopy(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<GetCopyAllExportLayoutForViewDto>();
            var data = new List<GetCopyAllExportLayoutForViewDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        data.Add(new GetCopyAllExportLayoutForViewDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),                            
                            Description = dataReader["cDescription"].ToString().Trim(),
                            
                        });
                    }
                }
                result.Items = data;
            }
            return result;
        }
    }
}
