using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.IDMSTasks;
using Infogroup.IDMS.IDMSTasks.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.IDMSTask
{
    public class IDMSTaskRepository : IDMSRepositoryBase<IDMSTasks.IDMSTask, int>, IIDMSTaskRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public IDMSTaskRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public int GetDivionIdFromDatabase(int databaseId)
        {
            _databaseHelper.EnsureConnectionOpen();

            using (var command = _databaseHelper.CreateCommand($@" select db.DivisionID  from 
                                                                    dbo.tblDatabase db Inner Join dbo.tblDivision div on db.DivisionID = div.ID 
                                                                    where db.ID = {databaseId} ", CommandType.Text))

            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public DataSet CheckTableName(string connectionString, string sbNewTableName)
        {
            var ds = new DataSet();
            var sql = "SELECT reverse(left(reverse(name), charindex('_', reverse(name)) -1)) AS ListID,Name FROM sys.tables with(nolock) where name in (" + sbNewTableName + ")";

            using (var DA = new SqlDataAdapter(sql, connectionString))
            {
                DA.Fill(ds);

                return ds;
            }
        }

        public int RenameTables(string connectionString, string oldTableName, string NewTableName)
        {
            var result = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }                  

                    using (var cmd = new SqlCommand("sys.sp_rename", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@objname", oldTableName).SqlDbType = SqlDbType.NVarChar;
                        cmd.Parameters.AddWithValue("@newname", NewTableName).SqlDbType = SqlDbType.NVarChar;
                        result = cmd.ExecuteNonQuery();

                        if (con.State != ConnectionState.Closed)
                        {
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            
            return result;
        }

        public int GetBuildLolID(int BuildID, string ListID)
        {
            _databaseHelper.EnsureConnectionOpen();

            using (var command = _databaseHelper.CreateCommand($@" Select TOP 1 BL.ID FROM tblBuildLoL BL WITH(NOLOCK)
                                                                WHERE BL.BuildID = {BuildID} AND BL.MasterLoLID = {ListID}", CommandType.Text))

            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void DropTableTORename(string connection, string tablename)
        {
            var sql = "";
            var listTable = tablename.Split(',');
            var Append = "ToBeDropped";
            var builder = new System.Text.StringBuilder();
            builder.Append(sql);
            for (int i = 0; i < listTable.Length; i++)
            {
                builder.Append(@" IF OBJECT_ID('" + listTable[i].Replace("'", "") + "' , 'U') IS NOT NULL  exec sys.sp_rename '" + listTable[i].Replace("'", "") + "','" + listTable[i].Replace("'", "") + "_" + Append + "'");
            }
            sql = builder.ToString();
            using (var conn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new UserFriendlyException(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public void TaskLoadMailerUsuage(LoadMailerUsageDto input)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@tnOldBuildID", input.TaskGeneral.BuildID),
                new SqlParameter("@tnNewBuildID", input.NewBuildID),
                new SqlParameter("@tcFileName", input.FilePath)
            };

            using (var command = _databaseHelper.CreateCommand("usp_LoadMailerUsage", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }
        }

        public BuildDetails GetDivisionIdFromDatabaseAndBuild(int buildId ,int databaseId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var details = new BuildDetails();
            using (var command = _databaseHelper.CreateCommand($@" SELECT tblBuild.ID, tblBuild.DatabaseID, tblBuild.cBuild, D.DivisionID from tblBuild  WITH (NOLOCK) INNER JOIN tblDatabase D WITH (NOLOCK) on tblBuild.DatabaseID= D.ID
                                                                Where tblBuild.ID ={buildId} and D.ID={databaseId}", CommandType.Text))

            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        details.Id = Convert.ToInt32(reader["ID"]);
                        details.DatabaseID = Convert.ToInt32(reader["DatabaseID"]);
                        details.BuildID = Convert.ToInt32(reader["cBuild"]);
                        details.DivisionID = Convert.ToInt32(reader["DivisionID"]);
                    }
                }
            }

            return details;
        }

        public string GetFieldName(int buildId)
        {
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand($@" Select TOP 1  BTL.cFieldName FROM tblBuildTable BT with(nolock) inner join tblBuildTableLayout BTL with(nolock) ON BT.ID = BTL.BuildTableID  
                                                                    where BT.BuildID = {buildId} and BTL.iKeyColumn = 1 ", CommandType.Text))

            {
                return command.ExecuteScalar().ToString();
            }
        }

        public BuildCopyDto ValidateCopyBuild(int TargetBuildId, int SourceBuildId)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var build = new BuildCopyDto();
                using (var command = _databaseHelper.CreateCommand($@"SELECT BT.DatabaseID, BT.iIsReadyToUse from tblBuild BT with(nolock) where ID = {TargetBuildId}
                                                                        and BT.DatabaseID  = (Select DatabaseID FROM tblBuild BS with(nolock) WHERE BS.ID  = {SourceBuildId} )", CommandType.Text))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            build.iIsReadyToUse = Convert.ToBoolean(reader["iIsReadyToUse"]);
                            build.DatabaseId = Convert.ToInt32(reader["DatabaseId"]);
                        }
                    }
                }
                return build;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetExportFlagFields(int buildID)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var fields = new List<DropdownOutputDto>();
                using (var command = _databaseHelper.CreateCommand($@"select tBuild.cFieldName,tBuild.cFieldDescription from tblBuildTable tb,tblBuildTableLayout tBuild
                          where tb.ID = tBuild.BuildTableID AND tb.BuildID = {buildID} AND tb.LK_TableType = 'M' Order By tBuild.cFieldDescription", CommandType.Text))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fields.Add(new DropdownOutputDto
                            {
                                Value = reader["cFieldName"].ToString(),
                                Label = reader["cFieldDescription"].ToString()
                            });
                           
                        }
                    }
                }
                return fields;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetAllTableDescription(string buildID)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var fields = new List<DropdownOutputDto>();
                using (var command = _databaseHelper.CreateCommand($@"SELECT cTableName,ctabledescription FROM tblBuildTable WITH (NOLOCK) 
                                                                WHERE BuildId= {buildID} ORDER BY ctabledescription DESC", CommandType.Text))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fields.Add(new DropdownOutputDto
                            {
                                Value = reader["cTableName"].ToString(),
                                Label = reader["ctabledescription"].ToString()
                            });

                        }
                    }
                }
                return fields;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public int GetDataSetDatabaseByBuildID(int BuildID)
        {
            _databaseHelper.EnsureConnectionOpen();

            using (var command = _databaseHelper.CreateCommand($@" Select tblDatabase.ID from tblDatabase 
                                                                inner join tblBuild on tblBuild.DatabaseID = tblDatabase.ID and tblBuild.ID =  {BuildID}", CommandType.Text))

            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public List<GetAllListForBuildDto> GetAllListForBuild(int BuildID)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var lists = new List<GetAllListForBuildDto>();
                using (var command = _databaseHelper.CreateCommand($@" Select ID,BuildID,MasterLoLID,LK_Action,cSlugDate,cBatchDateType,LK_SlugDateType,iQuantityTotal,CAST(BuildID as VARCHAR(10))+CAST(MasterLoLID  as VARCHAR(10)) as cOnePassFileName,dCreatedDate,cCreatedBy 
                                                                    from tblBuildLoL WITH (NOLOCK) Where BuildID={BuildID} Order By LK_Action", CommandType.Text))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var list = new GetAllListForBuildDto
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                BuildID = Convert.ToInt32(reader["BuildID"]),
                                MasterLolID = Convert.ToInt32(reader["MasterLoLID"]),
                                LK_Action = Convert.ToChar(reader["LK_Action"]),
                                cSlugDate = reader["cSlugDate"].ToString().Trim(),
                                cBatchDateType = Convert.ToChar(reader["cBatchDateType"]),
                                LK_SlugDateType = Convert.ToChar(reader["LK_SlugDateType"]),
                                iQuantityTotal = Convert.ToInt32(reader["iQuantityTotal"]),
                                cOnePassFileName = reader["cOnePassFileName"].ToString().Trim(),
                                cCreatedBy = reader["cCreatedBy"].ToString().Trim(),
                                dCreatedDate = Convert.ToDateTime(reader["dCreatedDate"])
                            };

                            lists.Add(list);
                        }
                    }
                }
                return lists;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
