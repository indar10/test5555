using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.MatchAppends.Dtos;
using Infogroup.IDMS.SegmentSelections.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.BuildTableLayouts
{
    public class BuildTableLayoutRepository : IDMSRepositoryBase<BuildTableLayout, int>, IBuildTableLayoutRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private const string Space = " ";

        private const string sNoLock = " WITH (NOLOCK) ";
        public BuildTableLayoutRepository(IDbContextProvider<IDMSDbContext> dbContextProvider,
            IActiveTransactionProvider transactionProvider,
            DatabaseHelper.DatabaseHelper databaseHelper,
            IRedisIDMSConfigurationCache idmsConfigurationCache)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
            _idmsConfigurationCache = idmsConfigurationCache;
        }

        public int GetBuildTableLayoutID(int iBuildID, string sFieldName, string cTableName)
        {
            _databaseHelper.EnsureConnectionOpen();
            int result = 0;
            using (var command = _databaseHelper.CreateCommand(@"SELECT BTL.ID, BTL.cDataType FROM tblBuildTableLayout BTL WITH(NOLOCK) INNER JOIN tblBuildTable BT WITH(NOLOCK) ON BTL.BuildTableID = BT.ID WHERE BT.BuildID =" + iBuildID + " AND BT.cTableName = '" + cTableName + "'" + "AND BTL.cFieldName = '" + sFieldName + "'", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }


        public List<FieldData> GetFieldsForBuildLayout(int buildId, int mailerId, int iBuildLoLID, string sFileSpecific)
        {
            _databaseHelper.EnsureConnectionOpen();

            var sListCondition2 = Space;
            var sListCondition1 = Space;
            var sListCondition3 = Space;

            if (sFileSpecific == "2")
                sListCondition2 = "AND (iShowListBox = 1 OR iShowTextBox = 1) ";

            if (!iBuildLoLID.Equals(0))
                sListCondition3 = " AND ( (cFieldName LIKE 'RAW_Field%' AND BR.ID is NOT NULL) OR (cFieldName NOT LIKE 'RAW_Field%')) ";

            var lcSQL = $@"SELECT BL.ID,iDataLength,cFieldName,cTableName,
                                    CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '1' ELSE iShowTextBox END  as iShowTextBox,
                                    CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '0' ELSE iShowListBox END  as iShowListBox,
                                    CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '0' ELSE iFileOperations END  as iFileOperations,
                                    CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN 1 ELSE iShowDefault END  as iShowDefault, 
                                    cFieldType,
                                    iDisplayOrder,
                                    iIsListSpecific, 
                                    CASE ISNULL(BR.ID,0) WHEN 0 THEN BL.cFieldDescription  ELSE BR.cRAWFieldName END  AS cFieldDescription,
                                    LEFT(cTableName,charindex('_',cTableName,0)-1)+'.'+cFieldName AS CombinedField                            
                                    FROM tblBuildTableLayout BL WITH(NOLOCK) 
                                    INNER JOIN  tblBuildTable BT WITH(NOLOCK) 
                                    ON BL.BuildTableID = BT.ID 
                                    LEFT OUTER JOIN tblBuildTableLayoutMailer BLM WITH(NOLOCK) 
                                    ON BLM.MailerID={mailerId} AND  BLM.BuildTableLayoutID=BL.ID
                                    LEFT JOIN tblBuildRAWFieldName BR WITH(NOLOCK)
                                    ON BR.BuildTableLayoutID = BL.ID AND BR.BuildLoLID = {iBuildLoLID}
                                    WHERE BL.iIsSelectable = 1
                                    AND BT.BuildId={buildId}                                    
                                    {sListCondition1}
                                    {sListCondition2}
                                    AND ( BL.iIsMailerSpecific=0 or BLM.ID IS NOT NULL)
                                    {sListCondition3}
                                    ORDER BY BT.ID , BL.iDisplayOrder, BL.cFieldDescription ";

            var fieldsForBuildLayout = new List<FieldData>();
            using (var command = _databaseHelper.CreateCommand(lcSQL, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;


                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        fieldsForBuildLayout.Add(new FieldData
                        {
                            ID = Convert.ToInt32(dataReader["ID"]),
                            iDataLength = Convert.ToInt32(dataReader["iDataLength"]),
                            cFieldName = dataReader["cFieldName"].ToString(),
                            cTableName = dataReader["cTableName"].ToString(),
                            iShowTextBox = Convert.ToBoolean(dataReader["iShowTextBox"]),
                            iShowListBox = Convert.ToBoolean(dataReader["iShowListBox"]),
                            iFileOperations = Convert.ToBoolean(dataReader["iFileOperations"]),
                            iShowDefault = Convert.ToInt32(dataReader["iShowDefault"]),
                            cFieldType = dataReader["cFieldType"].ToString(),
                            iDisplayOrder = Convert.ToInt32(dataReader["iDisplayOrder"]),
                            iIsListSpecific = Convert.ToBoolean(dataReader["iIsListSpecific"]),
                            cFieldDescription = dataReader["cFieldDescription"].ToString()

                        });
                    }
                }

            }

            return fieldsForBuildLayout;
        }

        public List<string> GetFavouriteFields(int databaseId, int userId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new List<string>();
            var sqlQuery = $@"SELECT cFieldName,cTableNamePrefix FROM tblUserSelection WITH(NOLOCK) WHERE DatabaseID = {databaseId}  AND UserID = {userId}
            ORDER BY iDisplayOrder";
            using (var command = _databaseHelper.CreateCommand(sqlQuery, CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add($"{dataReader["cFieldName"]}{dataReader["cTableNamePrefix"]}");
                    }
                }
            }
            return result;
        }


        public FieldDetails GetSingleFieldDetails(int buildId, int mailerId, int iBuildLoLID, string sFileSpecific, string ID, int iDBID)
        {
            _databaseHelper.EnsureConnectionOpen();
            StringBuilder lcSQL = new StringBuilder();
            if (iDBID > 0)
            {
                //Field ID belongs to external DB
                lcSQL
                    .Append(@"SELECT TOP 1 BL.ID,iDataLength,cFieldName,cTableName ,
                        iShowTextBox,
                        iShowListBox,
                        iFileOperations,
                        iShowDefault, 
                        cFieldType,
                        iDisplayOrder,
                        iIsListSpecific
                        FROM tblBuildTableLayout BL WITH (NOLOCK) INNER JOIN  tblBuildTable BT WITH (NOLOCK) on  BT.ID = BL.BuildTableID
                        WHERE BL.ID = ")
                    .Append(ID);
            }
            else
            {
                string sListCondition2 = Space;
                string sListCondition1 = Space;
                string sListCondition3 = Space;

                if (sFileSpecific == "2")
                    sListCondition2 = "AND (iShowListBox = 1 OR iShowTextBox = 1) ";

                if (!iBuildLoLID.Equals(0))
                {
                    sListCondition3 = " AND ( (cFieldName LIKE 'RAW_Field%' AND BR.ID is NOT NULL) OR (cFieldName NOT LIKE 'RAW_Field%')) ";
                }

                lcSQL.Append($@"SELECT TOP 1 BL.ID,iDataLength,cFieldName,cTableName ,
                            CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '1' ELSE iShowTextBox END  as iShowTextBox,
                            CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '0' ELSE iShowListBox END  as iShowListBox,
                            CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '0' ELSE iFileOperations END  as iFileOperations,
                            CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN 1 ELSE iShowDefault END  as iShowDefault, 
                            cFieldType,
                            iDisplayOrder,
                            iIsListSpecific
                            FROM tblBuildTableLayout BL WITH (NOLOCK) INNER JOIN  tblBuildTable BT WITH (NOLOCK) on
                            BL.BuildTableID = BT.ID 
                            LEFT Outer join tblBuildTableLayoutMailer BLM WITH (NOLOCK) on BLM.MailerID={mailerId} and BLM.BuildTableLayoutID=BL.ID
                            Left Join tblBuildRAWFieldName BR WITH (NOLOCK) on BR.BuildTableLayoutID = BL.ID 
                            AND BR.BuildLoLID = ").Append(iBuildLoLID).Append("   WHERE BL.iIsSelectable =1 ");
                lcSQL.Append(sListCondition1).Append(sListCondition2);
                lcSQL.Append("AND ( BL.iIsMailerSpecific=0 or BLM.ID IS NOT NULL) AND BT.BuildID  = ").Append(buildId).Append(" AND BL.ID = ").Append(ID);
                lcSQL.Append(sListCondition3);
            }
            var fieldDetails = new FieldDetails();
            using (var command = _databaseHelper.CreateCommand(lcSQL.ToString(), CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        fieldDetails = new FieldDetails
                        {
                            ID = Convert.ToInt32(dataReader["ID"]),
                            iDataLength = Convert.ToInt32(dataReader["iDataLength"]),
                            cFieldName = dataReader["cFieldName"].ToString(),
                            cTableName = dataReader["cTableName"].ToString(),
                            iShowTextBox = Convert.ToBoolean(dataReader["iShowTextBox"]),
                            iShowListBox = Convert.ToBoolean(dataReader["iShowListBox"]),
                            iFileOperations = Convert.ToBoolean(dataReader["iFileOperations"]),
                            iShowDefault = Convert.ToInt32(dataReader["iShowDefault"]),
                            cFieldType = dataReader["cFieldType"].ToString(),
                            iDisplayOrder = Convert.ToInt32(dataReader["iDisplayOrder"]),
                            iIsListSpecific = Convert.ToBoolean(dataReader["iIsListSpecific"]),

                        };
                    }
                }
            }
            return fieldDetails;
        }


        public GetExportLayoutSelectedFieldsDto GetFieldName(string field, int tableId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var test = new GetExportLayoutSelectedFieldsDto();
            using (var command = _databaseHelper.CreateCommand($@" SELECT * FROM tblBuildTableLayout BTL  WHERE BTL.cFieldDescription ='{field}' AND BTL.BuildTableID = {tableId}", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;


                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        test = new GetExportLayoutSelectedFieldsDto { Width = Convert.ToInt32(dataReader["iDataLength"]), fieldName = dataReader["cFieldName"].ToString() };


                    }
                }
            }
            return test;

        }

        public GetMatchAppendFieldDetails GetMatchAppendFieldDetails(string field, int tableId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var obj = new GetMatchAppendFieldDetails();
            using (var command = _databaseHelper.CreateCommand($@" SELECT * FROM tblBuildTableLayout BTL  WHERE BTL.cFieldName = '{field}' AND BTL.BuildTableID = {tableId}", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;


                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        obj = new GetMatchAppendFieldDetails { cWidth = dataReader["iDataLength"].ToString(), cFieldDescription = dataReader["cFieldDescription"].ToString() };


                    }
                }
            }
            return obj;

        }


        public bool CheckMaxPerFields(int campaignId, string configItem)
        {
            _databaseHelper.EnsureConnectionOpen();
            var lcSQL = new StringBuilder();
            lcSQL.Append(@"if exists(SELECT 1
                           FROM tblBuildTableLayout BL INNER JOIN  tblBuildTable BT on
                           BL.BuildTableID = BT.ID and LK_TableType='M' Left Join tblBuildRAWFieldName BR on BR.BuildTableLayoutID = BL.ID  AND (BR.BuildLoLID = 0 OR BR.BuildLoLID is null)  WHERE BL.iAllowMaxPer = 1  AND BL.iIsListSpecific=0  AND BT.BuildID IN ( " + campaignId + " )and  cFieldName = '" + configItem + "')");
            lcSQL.Append(" select 1 else select 0");
            using (var command = _databaseHelper.CreateCommand(lcSQL.ToString(), CommandType.Text))
            {
                return (Convert.ToInt32(command.ExecuteScalar()) > 1);
            }
        }
        public async Task<Field> GetFieldDetailByName(string BuildID, string TablePrefix, string FieldName, int DatabaseID)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new Field { cQuestionFieldName = FieldName, cValueMode = "T" };
            var sb = new StringBuilder();
            sb.AppendLine(@"Select BTL.ID as FieldID , BT.cTableName,BTL.cFieldDescription,
                            CASE WHEN BTL.iShowDefault=2 THEN 'G' ELSE 'T' END AS ShowDefault
                            FROM tblBuildTable BT with(nolock) inner join tblBuildTableLayout BTL WITH(NOLOCK)
                            on BT.Id = BTL.BuildTableID ");
            sb.AppendFormat(" WHERE BT.BuildID = {0} AND ", BuildID);
            sb.AppendFormat(" substring(BT.cTableName,0,charindex('_',BT.cTableName)) = '{0}'", TablePrefix);
            sb.AppendFormat(" AND BTL.cFieldName = '{0}'", FieldName);
            sb.AppendLine(" UNION ");
            sb.AppendLine(@"Select BTL.ID as FieldID , BT.cTableName,BTL.cFieldDescription,
                            CASE WHEN BTL.iShowDefault=2 THEN 'G' ELSE 'T' END AS ShowDefault
                            FROM tblBuildTable BT with(nolock) inner join tblBuildTableLayout BTL WITH(NOLOCK)
                            on BT.Id = BTL.BuildTableID ");
            sb.AppendFormat(" INNER JOIN tblExternalBuildTableDatabase ExtDB WITH(NOLOCK) ON ExtDB.BuildTableID = BT.ID ");
            sb.AppendFormat(" WHERE ExtDB.DatabaseID = {0} AND ", DatabaseID);
            sb.AppendFormat(" substring(BT.cTableName,0,charindex('_',BT.cTableName)) = '{0}'", TablePrefix);
            sb.AppendFormat(" AND BTL.cFieldName = '{0}'", FieldName);
            using (var command = _databaseHelper.CreateCommand(sb.ToString(), CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        result.ID = Convert.ToInt32(dataReader["FieldID"]);
                        result.cValueMode = dataReader["ShowDefault"].ToString();
                        result.cQuestionDescription = dataReader["cFieldDescription"].ToString();
                        result.cTableName = dataReader["cTableName"].ToString();
                    }
                }
            }
            return result;
        }
        public async Task<List<DropdownOutputDto>> GetValues(int sBuildLayoutID)
        {
            var result = new List<DropdownOutputDto>();
            _databaseHelper.EnsureConnectionOpen();
            var sReplaceDescription = "REPLACE(REPLACE(DD.cDescription,'\"',''),'\\','') as cDescription ";
            var lcSQL = "SELECT DD.ID, DD.cValue," + sReplaceDescription + "  FROM tblBuildDD DD " + sNoLock + @" WHERE DD.BuildTableLayoutID=" + sBuildLayoutID + " AND DD.BuildLoLID=0 order by DD.iDisplayOrder, DD.cValue";
            using (var command = _databaseHelper.CreateCommand(lcSQL, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (await dataReader.ReadAsync())
                    {
                        result.Add(new DropdownOutputDto
                        {
                            Value = dataReader["cValue"].ToString(),
                            Label = dataReader["cDescription"].ToString()
                        });
                    }
                }
            }
            return result;
        }


        public List<GetBuildTableLayoutForViewDto> GetAllMultiFieldsData(GetAllBuildTableLayoutsInput input)
        {
            _databaseHelper.EnsureConnectionOpen();
            var multiFields = new List<GetBuildTableLayoutForViewDto>();
            var excludeFields = string.Empty;
            var excludeFieldConfigurationItem = _idmsConfigurationCache.GetConfigurationValue("ExcludeFromMultiFieldSearch", input.DatabaseId);
            if (!string.IsNullOrEmpty(excludeFieldConfigurationItem.cValue))
            {
                excludeFields = string.Concat("'", excludeFieldConfigurationItem.cValue.Trim().Replace(",", "','"), "'");
            }

            using (var command = _databaseHelper.CreateCommand($@"SELECT DISTINCT BL.ID, DD.cDescription, DD.cValue,
            cFieldDescription, cTableName, cFieldName  FROM tblBuildTableLayout BL WITH (NOLOCK) 
            INNER JOIN tblBuildTable BT WITH (NOLOCK) ON BL.BuildTableID = BT.ID
            INNER JOIN tblBuildDD DD WITH (NOLOCK) ON DD.BuildTableLayoutID = BL.ID
            LEFT OUTER JOIN tblBuildTableLayoutMailer BLM WITH (NOLOCK) ON BLM.MailerID = {input.MailerId}
	        AND BLM.BuildTableLayoutID = BL.ID
            LEFT JOIN tblBuildRAWFieldName BR WITH (NOLOCK) ON BR.BuildTableLayoutID = BL.ID
	        AND BR.BuildLoLID = 0  WHERE BL.iIsSelectable = 1 AND DD.BuildLoLID = 0 
	        AND (
		     BL.iIsMailerSpecific = 0
		     OR BLM.ID IS NOT NULL
		     )
	         AND BL.iIsListSpecific = 0 And (cFieldDescription Like @Filter OR DD.cDescription Like 
            @Filter OR DD.cValue Like @Filter) AND BT.BuildID = {input.BuildId}  
             AND cFieldName NOT IN ( @ExcludedFields ) 
             UNION  SELECT DISTINCT BL.ID, DD.cDescription, DD.cValue, cFieldDescription, cTableName, cFieldName
             FROM tblExternalBuildTableDatabase ExDB WITH (NOLOCK)
             INNER JOIN tblBuildTable BT WITH (NOLOCK) ON BT.ID = exDB.BuildTableID
             INNER JOIN tblBuildTableLayout BL WITH (NOLOCK) ON BL.BuildTableID = BT.ID
             INNER JOIN tblBuildDD DD WITH (NOLOCK) ON DD.BuildTableLayoutID = BL.ID
             WHERE DD.BuildLoLID = 0
	         AND BL.iIsListSpecific = 0 And (cFieldDescription Like  @Filter OR DD.cDescription Like 
           @Filter OR DD.cValue Like @Filter) AND ExDB.DatabaseID = {input.DatabaseId}   
             AND cFieldName NOT IN ( @ExcludedFields )  
             ORDER BY cFieldDescription,cValue,cDescription ; select @@ROWCOUNT; ", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;
                command.Parameters.Add(new SqlParameter("@Filter", $"%{input.Filter}%"));
                command.Parameters.Add(new SqlParameter("@ExcludedFields", excludeFields));

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        multiFields.Add(
                            new GetBuildTableLayoutForViewDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                cTableName = Convert.ToString(dataReader["cTableName"]),
                                cFieldName = Convert.ToString(dataReader["cFieldName"]),
                                cDescription = Convert.ToString(dataReader["cDescription"]),
                                cFieldDescription = Convert.ToString(dataReader["cFieldDescription"]),
                                cValue = Convert.ToString(dataReader["cValue"])
                            });
                    }
                }
            }
            return multiFields;
        }


        public async Task<List<DropdownOutputDto>> GetFindReplaceFields(int buildId, int databaseId , int mailerId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new List<DropdownOutputDto>();
            var sqlQuery = $@"SELECT BL.ID,
                                     BL.cFieldName,
                                     BL.cFieldDescription
                              FROM tblBuildTableLayout BL WITH (NOLOCK)
                              INNER JOIN tblBuildTable BT WITH (NOLOCK) ON BL.BuildTableID = BT.ID
                              LEFT OUTER JOIN tblBuildTableLayoutMailer BLM WITH (NOLOCK) ON BLM.MailerID={mailerId}
                              AND BLM.BuildTableLayoutID=BL.ID
                              WHERE BL.iIsSelectable = 1
                              AND (BL.iIsMailerSpecific= 0 OR BLM.ID IS NOT NULL)
                              AND (BL.iShowTextBox= 1 OR BL.iShowListBox= 1)
                              AND BT.BuildID = {buildId}
                              UNION
                              SELECT BL.ID,
                              BL.cFieldName,
                              BL.cFieldDescription
                              FROM tblBuildTable BT WITH (NOLOCK)
                              INNER JOIN tblBuildTableLayout BL WITH (NOLOCK) ON BL.BuildTableID = BT.ID
                              INNER JOIN
                                (SELECT DISTINCT BuildTableID FROM tblExternalBuildTableDatabase WITH (NOLOCK) WHERE DatabaseID = {databaseId} ) ExtDB 
                              ON BT.id = ExtDB.BuildTableID
                             WHERE BL.iIsSelectable = 1
                             ORDER BY BL.cFieldDescription;";
            using (var command = _databaseHelper.CreateCommand(sqlQuery, CommandType.Text))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        var cFieldDescription = dataReader["cFieldDescription"].ToString();
                        result.Add(new DropdownOutputDto
                        {
                            Label = cFieldDescription,
                            Value = new Field
                            {
                                cQuestionDescription = cFieldDescription,
                                cQuestionFieldName = dataReader["cFieldName"].ToString(),
                                ID = Convert.ToInt32(dataReader["ID"])
                            }
                        });
                    }
                }
            }
            return result;
        }

        public List<DropdownOutputDto> GetBuildTableLayoutFieldByBuildID(string iBuildID)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new List<DropdownOutputDto>();

            result.Add(new DropdownOutputDto { Label = "Select...", Value = "" });
            var query = $@"select tBuild.cFieldName,tBuild.cFieldDescription from tblBuildTable tb,tblBuildTableLayout tBuild
                          where tb.ID = tBuild.BuildTableID AND tb.BuildID = {iBuildID} AND tb.LK_TableType = 'M' Order By tBuild.cFieldDescription";

            using (var command = _databaseHelper.CreateCommand(query, CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new DropdownOutputDto
                        {
                            Value = dataReader["cFieldName"].ToString(),
                            Label = dataReader["cFieldDescription"].ToString()

                        });
                    }
                }

            }
            return result;
        }
        public BuildTableLayoutDto GetExportableField(string fieldName,int BuildTableId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var build = new BuildTableLayoutDto();
            using (var command = _databaseHelper.CreateCommand($@"SELECT B.ID ,
                                                                   B.cFieldName,B.iAllowExport                                                                   
                                                                    FROM tblBuildTableLayout B 
                                                                    WHERE B.cFieldName = '{fieldName}' 
                                                                    And B.BuildTableID= {BuildTableId} ", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        build.cFieldName = dataReader["cFieldName"].ToString();
                        build.iAllowExport = Convert.ToBoolean(dataReader["iAllowExport"]);
                        
                    }
                }
            }
            return build;


        }
       
    }
}
