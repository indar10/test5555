using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.BuildTables.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.BuildTables
{
    public class BuildTableRepository : IDMSRepositoryBase<BuildTable, int>, IBuildTableRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public BuildTableRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public List<BuildTableDto> GetExternalTables(int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var outputlayoutTemplete = new List<BuildTableDto>();

            using (var command = _databaseHelper.CreateCommand($@"Select tblBuildTable.* 
                         FROM tblOrder 
                           INNER JOIN tblBuild ON tblBuild.ID = tblOrder.BuildID
                          INNER JOIN tblExternalBuildTableDatabase ExDB on ExDB.DatabaseID = tblBuild.DatabaseID
                          INNER JOIN  tblBuildTable ON tblBuildTable.ID = ExDB.BuildTableID
                            WHERE tblOrder.ID = {campaignId}", CommandType.Text))

            {

                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        outputlayoutTemplete.Add(new BuildTableDto { Id = Convert.ToInt32(dataReader["ID"]), cTableName = dataReader["cTableName"].ToString(), ctabledescription = dataReader["ctabledescription"].ToString()});
                    }
                }
            }
            return outputlayoutTemplete;
        }

        public List<BuildTableDto> GetExternalTablesByDatabase(int databseId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var outputlayoutTemplete = new List<BuildTableDto>();

            using (var command = _databaseHelper.CreateCommand($@"SELECT DISTINCT tblbuildtable.* 
                            FROM   tblbuild WITH(NOLOCK)
                            INNER JOIN tblexternalbuildtabledatabase ExDB WITH(NOLOCK)
                                ON ExDB.databaseid = tblbuild.databaseid 
                            INNER JOIN tblbuildtable WITH(NOLOCK)
                                ON tblbuildtable.id = ExDB.buildtableid 
                            WHERE  tblbuild.databaseid = {databseId}", CommandType.Text))

            {

                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        outputlayoutTemplete.Add(new BuildTableDto { Id = Convert.ToInt32(dataReader["ID"]), cTableName = dataReader["cTableName"].ToString(), ctabledescription = dataReader["ctabledescription"].ToString() });
                    }
                }
            }
            return outputlayoutTemplete;
        }
        public List<GetExportLayoutAddFieldsDto> GetExportLayoutAddFields(int tableId,int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var outputlayoutTemplete = new List<GetExportLayoutAddFieldsDto>();

            using (var command = _databaseHelper.CreateCommand($@"SELECT BTL.cFieldName, BTL.cFieldDescription,CASE WHEN BT.LK_TableType = 'M' THEN 'MainTable' ELSE  Left(BT.cTableName,CHARINDEX('_',BT.cTableName)-1) END + cFieldName AS [FieldTableKey]
                        FROM tblBuildTableLayout BTL with(nolock) 
                        inner join tblBuildTable BT with(nolock) ON BT.ID = BTL.BuildTableID
                        WHERE BTL.iAllowExport = 1 
						AND BTL.BuildTableID = {tableId}
						 AND CASE WHEN BT.LK_TableType = 'M' THEN 'MainTable' ELSE  Left(BT.cTableName,CHARINDEX('_',BT.cTableName)-1) END
                            + cFieldName  
                                NOT IN (Select cTableNamePrefix+cFieldName FROM tblOrderExportLayout with(nolock) WHERE OrderID = {campaignId})
                            ORDER BY BTL.cFieldDescription;", CommandType.Text))

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


        public BuildTableDto CheckTableDecriptionOfExcelSheet(int buildId, string tableDescription)
        {

            _databaseHelper.EnsureConnectionOpen();
            var build = new BuildTableDto();
            //var query = $@"SELECT B.ID ,B.cFieldName,B.iAllowExport FROM tblBuildTableLayout B WITH (NOLOCK) WHERE B.cFieldName = '{fieldName}'";
            using (var command = _databaseHelper.CreateCommand($@"
                                                                SELECT B.ID , B.cTableName,B.ctabledescription                                                                                                                                   
                                                                    FROM tblBuildTable B 
                                                                    WITH (NOLOCK)                                                 
                                                                    WHERE B.ctabledescription = '{tableDescription}' 
                                                                   And B.BuildID={buildId}", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        build.cTableName = dataReader["cTableName"].ToString();
                        build.Id = Convert.ToInt32(dataReader["ID"]);
                        build.ctabledescription = dataReader["ctabledescription"].ToString();
                    }
                }
            }
            return build;


        }
    }
}
