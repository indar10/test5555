using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.SegmentSelections.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SegmentSelections
{
    public class SegmentSelectionRepository : IDMSRepositoryBase<SegmentSelection, int>, ISegmentSelectionRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public SegmentSelectionRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }


        public int GetSubSelListCount(int iSubSelID)
        {
            _databaseHelper.EnsureConnectionOpen();
            int result = 0;
            string TableName = "TblSubSelectList";
            using (var command = _databaseHelper.CreateCommand(@" Select count(*) from " + TableName + " where SubSelectID = " + iSubSelID, CommandType.Text))
            {
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }

        public StringBuilder GetZipSelection(string sZipRadiusHashList)
        {
            StringBuilder sZip = new StringBuilder("");
            _databaseHelper.EnsureConnectionOpen();

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@ZipRadiusHashList", sZipRadiusHashList));

            using (var command = _databaseHelper.CreateCommand("usp_GetZipRadius", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        sZip.AppendLine(dataReader["ZipCode"].ToString());
                    }

                    return sZip;
                }

            }
        }
        public int GetSegmentListCount(int segmentId)
        {
            _databaseHelper.EnsureConnectionOpen();
            int result = 0;
            using (var command = _databaseHelper.CreateCommand(@"Select count(*) from tblsegment where ID = " + segmentId, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                result = Convert.ToInt32(command.ExecuteScalar());
            }

            return result;
        }
        public List<BindSegmentSelectionDetails> GetSegmentSelectionBySegmentID(int iSegmentID, int iBuildLoLID)
        {
            var result = new List<BindSegmentSelectionDetails>();
            var sIsRAWNotMapped = "CAST(0 as Bit) as iIsRAWNotMapped ";
            var tempVar = @"case cValueMode when 'F' then case BTL.cFieldName  when 'ZIPRADIUS' then cValues else cfilename end else cValues end as cValues,
                            case cValueMode when 'F' then case BTL.cFieldName  when 'ZIPRADIUS' then cDescriptions else cfilename end  else cDescriptions end as cDescription";

            if (iBuildLoLID == 0)
            {
                //Multple or 1 Lists
                //Text box as default            
            }
            else
            {
                //BuildLolID = single list
                //Show what ever was added            
                sIsRAWNotMapped = @" CAST(CASE 
                  WHEN (BTL.iIsListSpecific = 1 AND BR.ID is null AND cFieldName LIKE 'RAW_Field%')
                     THEN 1
                  ELSE 0 END AS  bit) as iIsRAWNotMapped ";
            }
            var lcSQL = string.Format(@"SELECT SS.ID,SS.cFileName as cFileName, SegmentID,cSystemFileName,cQuestionFieldName,cQuestionDescription,
                        CASE ISNULL(BR.ID,0) WHEN 0 THEN BTL.cFieldDescription  ELSE BR.cRAWFieldName END  AS Description ,
                        cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,
                        {0},SS.cCreatedBy, SS.dCreatedDate,
                        cValueMode,cValueOperator, BTL.ID as FieldID,BTL.iShowTextBox,BTL.iShowListBox,BTL.iFileOperations, BTL.iShowDefault, BTL.iIsListSpecific,  case BTL.cFieldName  when 'ZIPRADIUS' then '1' else '0' end as IsZipRadius, BTL.cFieldType , " + sIsRAWNotMapped + " FROM " + "tblSegmentSelection" +
                    @" SS WITH(NOLOCK) inner join tblSegment S WITH(NOLOCK) on  S.ID = SS.SegmentID  
                       inner join tblOrder O WITH(NOLOCK) on O.ID = S.OrderID 
                       inner join tblBuild B WITH(NOLOCK) on B.ID = O.BuildID 
                       left OUTER join tblBuildTable BT1 WITH(NOLOCK) on BT1.BuildID = B.ID   and BT1.cTableName = SS.cTableName
                       left OUTER join tblBuildTable BT2 WITH(NOLOCK) on BT2.ID IN (sELECT BT.ID
                       From tblExternalBuildTableDatabase ExDB WITH(NOLOCK)  inner join tblBuild WITH(NOLOCK) on ExDB.DatabaseID = tblBuild.DatabaseID
                       INNER join tblBuildTable BT WITH(NOLOCK) on BT.ID = ExDB.BuildTableID
                       WHERE tblBuild.ID = B.ID and BT.cTableName = SS.cTableName) 
                       left join tblBuildTableLayout  BTL WITH(NOLOCK) on ( BTL.BuildTableID = BT1.ID   OR  BTL.BuildTableID = BT2.ID  )
                       and 
                       (( SS.cQuestionFieldName = 'ZIP' and 
                       ((len(CONVERT(varchar(max), cValues) ) > 0 and  cValueMode = 'F' and BTL.cFieldName = 'ZIPRADIUS') OR
					   ((len(CONVERT(varchar(max), cValues) ) = 0 OR  cValueMode <> 'F') and BTL.cFieldName = 'ZIP' )))
					   OR ( SS.cQuestionFieldName  <> 'ZIP' AND BTL.cFieldName = SS.cQuestionFieldName ) )
                      Left Join tblBuildRAWFieldName BR WITH(NOLOCK) on BR.BuildTableLayoutID = BTL.ID AND BR.BuildLoLID = " + iBuildLoLID +
                       "WHERE SS.SegmentID =" + iSegmentID + @"  Order By iGroupNumber, SS.ID ", tempVar);

            using (var command = _databaseHelper.CreateCommand(lcSQL.ToString(), CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                     
                            result.Add(new BindSegmentSelectionDetails
                            {

                                selectionId = dataReader["ID"] is DBNull ? 0:Convert.ToInt32(dataReader["ID"]),
                                id = dataReader["FieldID"] is DBNull ? 0:Convert.ToInt32(dataReader["FieldID"]),
                                field = dataReader["cQuestionFieldName"] is DBNull ? string.Empty:Convert.ToString(dataReader["cQuestionFieldName"]),
                                Operator = dataReader["cValueOperator"] is DBNull ? string.Empty:Convert.ToString(dataReader["cValueOperator"]).Trim(),
                                value = dataReader["cValues"] is DBNull ? string.Empty:((Convert.ToString(dataReader["cValues"]).Contains("\\") || Convert.ToString(dataReader["cValues"]).Contains("//")) && Convert.ToString(dataReader["cValueMode"])=="F" && Convert.ToString(dataReader["cQuestionDescription"]) != "Zip Radius" ? Path.GetFileName(Convert.ToString(dataReader["cValues"])): Convert.ToString(dataReader["cValues"])),
                                iGroupNumber = dataReader["iGroupNumber"] is DBNull ? 0:Convert.ToInt32(dataReader["iGroupNumber"]),
                                cJoinOperator = dataReader["cJoinOperator"] is DBNull ? string.Empty: Convert.ToString(dataReader["cJoinOperator"]),
                                cValueMode = dataReader["cValueMode"] is DBNull ? string.Empty:Convert.ToString(dataReader["cValueMode"]),
                                cFileName = dataReader["cFileName"] is DBNull ? string.Empty : Convert.ToString(dataReader["cFileName"]),
                                cSystemFileName = dataReader["cSystemFileName"] is DBNull ? string.Empty : Convert.ToString(dataReader["cSystemFileName"]),
                                cGrouping = dataReader["cGrouping"] is DBNull ? string.Empty : Convert.ToString(dataReader["cGrouping"]),
                                iIsListSpecific = dataReader["iIsListSpecific"] is DBNull ? false : Convert.ToBoolean(dataReader["iIsListSpecific"]),
                                iIsRAWNotMapped = dataReader["iIsRAWNotMapped"] is DBNull ? false : Convert.ToBoolean(dataReader["iIsRAWNotMapped"]),
                                cCreatedBy = Convert.ToString(dataReader["cCreatedBy"]),
                                dCreatedDate = Convert.ToDateTime(dataReader["dCreatedDate"])

                            });
                        
                    }
                }
            }
            return result;
        }

        public bool DeleteRecords(List<int> listOfSegmentIds, int campaignId)
        {
            var SegIds = string.Join(",", listOfSegmentIds.ToArray());
            SegIds = SegIds.Replace(" ", string.Empty).Replace(",,", ",");
            if (listOfSegmentIds.Count > 0 && SegIds.Length > 0 && campaignId > 0)
            {
                var lSql = "Delete SS from tblSegmentSelection SS Inner Join tblSegment S WITH(NOLOCK) ON SS.SegmentID = S.ID WHERE S.ID IN (" + SegIds + ")";
                lSql += $" AND S.OrderID = {campaignId}; DELETE S from tblSegment S inner join tblOrder O WITH(NOLOCK) ON S.OrderID = O.ID WHERE O.ID = {campaignId} AND S.ID IN ({SegIds});";

                using (var command = _databaseHelper.CreateCommand(lSql, CommandType.Text))
                {
                    command.ExecuteNonQuery();
                    return true;
                }    
            }
            return false;
        }
        public List<SelectionDetails> GetSegmentSelectionByOrderID(int orderId, string fieldName, string TablePrefix = null)
        {
            var result = new List<SelectionDetails>();
            var lsSQL = new StringBuilder();
            lsSQL.Append($@"Select CASE WHEN (BTL.iShowListBox=1 OR BTL.iShowTextBox = 1) THEN 
                CASE WHEN BTL.iShowDefault=1 THEN 'T' 
                 ELSE 'G' END
                    ELSE'N'END AS ValueMode,BT.CtableName as TableName,BTL.cFieldDescription as QuestionDescription,BTL.cFieldType as cFieldType FROM tblBuildTableLayout BTL with(nolock) 
                             inner join tblBuildTable BT with(nolock) ON BTL.BuildTableID = BT.ID
                             inner join tblOrder o with(nolock) on o.BuildID= BT.BuildID
                             WHERE BTL.iIsSelectable = 1 AND BTL.cFieldName='{fieldName}' AND o.ID={orderId}");
            if (!string.IsNullOrEmpty(TablePrefix))
                lsSQL.Append($" AND BT.cTableName like '% {TablePrefix} %'");
            lsSQL.Append($@" UNION  SELECT CASE WHEN (BTL.iShowListBox=1 OR BTL.iShowTextBox = 1) THEN 
                CASE WHEN (BTL.iShowDefault=1) THEN 'T' 
                 ELSE 'G' END
              ELSE'N'END AS ValueMode,BT.CtableName as TableName,BTL.cFieldDescription as QuestionDescription ,BTL.cFieldType as cFieldType
              FROM tblBuildTableLayout BTL with(nolock) 
              inner join tblBuildTable BT with(nolock) ON BTL.BuildTableID = BT.ID  
              WHERE BT.ID in (
              SELECT ExtDB.BuildTableID
              FROM tblExternalBuildTableDatabase ExtDB WITH(NOLOCK) 
              INNER JOIN tblDatabase D WITH(NOLOCK) ON ExtDB.DatabaseID = D.ID
              INNER Join tblBuild B WITH(NOLOCK) ON B.DatabaseID = D.ID
              INNER join tblOrder O WITH(NOLOCK) ON O.BuildID = B.ID
              WHERE O.ID = {orderId} )
              AND BTL.iIsSelectable = 1 AND BTL.cFieldName='{fieldName}'");
            if (!string.IsNullOrEmpty(TablePrefix))
                lsSQL.Append($" AND BT.cTableName like '% {TablePrefix} %'");


            using (var command = _databaseHelper.CreateCommand(lsSQL.ToString(), CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new SelectionDetails
                        {
                            cFieldType = !string.IsNullOrEmpty(Convert.ToString(dataReader["cFieldType"]))? Convert.ToString(dataReader["cFieldType"]).ToUpper():string.Empty,
                            cTableName=  !string.IsNullOrEmpty(Convert.ToString(dataReader["TableName"]))? Convert.ToString(dataReader["TableName"]):string.Empty,
                            cQuestionDescription = !string.IsNullOrEmpty(Convert.ToString(dataReader["QuestionDescription"]))? Convert.ToString(dataReader["QuestionDescription"]):string.Empty,
                            cValueMode = !string.IsNullOrEmpty(Convert.ToString(dataReader["ValueMode"])) ? Convert.ToString(dataReader["ValueMode"]).ToUpper():string.Empty
                        });
                    }
                }
            }

            return result;
        }
        public string GetNotToBeUpperCasedFields(int databaseId)
        {

            _databaseHelper.EnsureConnectionOpen();
            var result = string.Empty;
            using (var command = _databaseHelper.CreateCommand(@" SELECT TOP 1 * FROM tblConfiguration 
                    WHERE cItem = 'NOT_TO_BE_UPPER_CASED' AND iIsActive = 1 AND (DatabaseID = 0 OR DatabaseID = " + databaseId +
                    ") ORDER BY DatabaseID DESC", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result = Convert.ToString(dataReader["cValue"]);
                    }
                }
            }
            return result;
        }

        public int AddSegmentSelection(SegmentSelectionDto segmentSelection, int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderID", campaignId),
                new SqlParameter("@SegmentID", segmentSelection.SegmentId),
                new SqlParameter("@QuestionFieldName", segmentSelection.cQuestionFieldName),
                new SqlParameter("@QuestionDescription", segmentSelection.cQuestionDescription),
                new SqlParameter("@JoinOperator", segmentSelection.cJoinOperator),
                new SqlParameter("@Values", segmentSelection.cValues),
                new SqlParameter("@ValueMode", segmentSelection.cValueMode),
                new SqlParameter("@Descriptions", segmentSelection.cDescriptions),
                new SqlParameter("@ValueOperator", segmentSelection.cValueOperator),
                new SqlParameter("@FileName", string.Empty),
                new SqlParameter("@Extension", string.Empty),
                new SqlParameter("@SystemFileName", string.Empty),
                new SqlParameter("@TableName", segmentSelection.cTableName),
                new SqlParameter("@InitiatedBy", segmentSelection.cCreatedBy),
                new SqlParameter("@SysFileName", DbType.String)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = string.Empty
                },
                new SqlParameter("@Identity", DbType.String)
                {
                    Direction = ParameterDirection.Output,
                    Value = 0
                }
            };
            using (var command = _databaseHelper.CreateCommand("usp_AddSegmentSelection", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
                return Convert.ToInt32(command.Parameters["@Identity"].Value);
            }
        }


        public void GroupSelection(int segmentID, int maxGroupId, string selections)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@SegmentID", segmentID));
            sqlParameters.Add(new SqlParameter("@SegmentSelectionList", selections));
            sqlParameters.Add(new SqlParameter("@SelectedGroup", maxGroupId));

            using (var command = _databaseHelper.CreateCommand("usp_SegmentGrouping", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }

        }
        public async Task<string> ApplyBulkOperations(SaveGlobalChangesInputDto input)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", input.Action),
                new SqlParameter("@OrderID", input.CampaignId),
                new SqlParameter("@UserID", input.UserID),
                new SqlParameter("@SourceSegment", input.SourceSegment == 0 ? DBNull.Value : (object)input.SourceSegment),
                new SqlParameter("@TargetSegments", (object)input.TargetSegments ?? DBNull.Value),
                new SqlParameter("@SearchValue", (object)input.SearchValue ?? DBNull.Value),
                new SqlParameter("@ReplaceValue", (object)input.ReplaceValue ?? DBNull.Value),
                new SqlParameter("@FieldName", (object)input.FieldName ?? DBNull.Value)
            };
            using (var command = _databaseHelper.CreateCommand("usp_OrderBulkOperations", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                var result = await command.ExecuteScalarAsync();
                return result.ToString();
            }
        }
    }
}
