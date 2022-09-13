using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.SegmentSelections.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.Builds
{
    public class BuildRepository : IDMSRepositoryBase<Build, int>, IBuildRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        private const string Space = " ";

        public BuildRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public List<ValueList> GetValues(string sBuildLayoutID, int iBuildLoLID, string sOrderBy, string sSortDirection)
        {
            _databaseHelper.EnsureConnectionOpen();
            sOrderBy = sOrderBy.Trim().Length == 0 ? " order by DD.iDisplayOrder, DD.cValue " : sOrderBy;
            sSortDirection = sSortDirection.Trim().Length == 0 ? " asc" : sSortDirection;
            string lcSQL;
            string sReplaceDescription = "REPLACE(REPLACE(DD.cDescription,'\"',''),'\\','') as cDescription ";
            if (iBuildLoLID == 0)
            {
                lcSQL = "SELECT DD.ID, DD.cValue," + sReplaceDescription + "  FROM tblBuildDD DD " + sNoLock + @" WHERE DD.BuildTableLayoutID=" + sBuildLayoutID + " AND DD.BuildLoLID=0 " + sOrderBy + " " + sSortDirection;
            }
            else
            {
                lcSQL = @"SELECT DD.ID, DD.cValue," + sReplaceDescription + " FROM tblBuildDD DD " + sNoLock + @" INNER JOIN tblBuildTableLayout BT " + sNoLock + @" ON DD.BuildTableLayoutID = BT.id 
                         WHERE DD.BuildTableLayoutID=" + sBuildLayoutID + " AND ((BT.iIsListSpecific= 1 and DD.BuildLoLID=" + iBuildLoLID + ") OR (BT.iIsListSpecific= 0 and DD.BuildLoLID=0)) " + sOrderBy + " " + sSortDirection;
            }
            var valueList = new List<ValueList>();
            using (var command = _databaseHelper.CreateCommand(lcSQL.ToString(), CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        valueList.Add(new ValueList
                        {
                            ID = Convert.ToInt32(dataReader["ID"]),
                            cValue = dataReader["cValue"].ToString(),
                            cDescription = dataReader["cDescription"].ToString()
                        });
                    }
                }

            }
            return valueList;
        }

        public BuildHierarchyDto GetBuildHierarchyDetails(int buildId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var build = new BuildHierarchyDto();
            using (var command = _databaseHelper.CreateCommand($@"SELECT B.ID as BuildID,
                                                                    B.cBuild as cBuild,
                                                                    D.ID as DatabaseID,
                                                                    D.DivisionID as DivisionID
                                                                    FROM tblBuild B 
                                                                    WITH (NOLOCK)
                                                                    INNER JOIN tblDatabase D
                                                                    WITH (NOLOCK)  
                                                                    ON B.DatabaseID = D.ID
                                                                    WHERE B.ID = {buildId}", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        build.BuildId = Convert.ToInt32(dataReader["BuildID"]);
                        build.cBuild = dataReader["cBuild"].ToString();
                        build.DatabaseId = Convert.ToInt32(dataReader["DatabaseID"]);
                        build.DivisionId = Convert.ToInt32(dataReader["DivisionID"]);
                    }
                }
            }
            return build;
        }

        public List<FieldData> GetExternalDatabaseFields(int databaseId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var objSQL = $@"SELECT  BT.id  as ExBTID, BL.ID,iDataLength,cFieldName,cTableName ,
                        iShowTextBox,
                        iShowListBox,
                        iFileOperations,
                        iShowDefault, 
                        cFieldType,
                        iDisplayOrder,
                        iIsListSpecific, 
                        cFieldDescription                           
                        FROM tblBuildTable BT WITH(NOLOCK) INNER JOIN tblBuildTableLayout BL WITH(NOLOCK) ON BL.BuildTableID = BT.ID 
                        INNER JOIN tblExternalBuildTableDatabase ExtDB WITH (NOLOCK) on BT.id = ExtDB.BuildTableID
                        WHERE BL.iIsSelectable =1 and ExtDB.DatabaseID = {databaseId} Order By ExBTID, BL.iDisplayOrder, BL.cFieldDescription ";
            var fieldDetails = new List<FieldData>();
            using (var command = _databaseHelper.CreateCommand(objSQL, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        fieldDetails.Add(new FieldData
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
                            cFieldDescription = dataReader["cFieldDescription"].ToString(),
                            iBTID = Convert.ToInt32(dataReader["ExBTID"])
                        });
                    }
                }
            }
            return fieldDetails;
        }

        public List<FieldData> GetExternalTablesByOrderIdForSubSelect(int databaseId,int mailerId, int iBuildLoLID, string sFileSpecific)
        {
            _databaseHelper.EnsureConnectionOpen();
            string sListCondition2 = Space;
            string sListCondition3 = Space;

            if (sFileSpecific == "2")
                sListCondition2 = " AND (iShowListBox = 1 OR iShowTextBox = 1) ";

            if (!iBuildLoLID.Equals(0))
            {
                sListCondition3 = " AND ( (cFieldName LIKE 'RAW_Field%' AND BR.ID is NOT NULL) OR (cFieldName NOT LIKE 'RAW_Field%')) ";
            }

            var lcSQL = new StringBuilder();
            lcSQL.Append($@"SELECT BR.BuildLoLID,BL.ID,iDataLength ,
		                cFieldName ,cTableName ,
		                CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '1' ELSE iShowTextBox END  AS iShowTextBox,
		                CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN  '0' ELSE iShowListBox END  AS iShowListBox,
		                CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '0' ELSE iFileOperations END   AS iFileOperations,
		                CASE WHEN (cFieldName LIKE 'RAW_Field%' AND BR.ID is NULL) THEN '1' ELSE iShowDefault END  AS iShowDefault,
		                cfieldtype as FieldDetail,
		                iDisplayOrder,iIsListSpecific, cDataType, BT.ID AS ExBTID,
		                CASE ISNULL(BR.ID,0) WHEN 0 THEN BL.cFieldDescription  ELSE BR.cRAWFieldName END  AS cFieldDescription
		            FROM tblBuildTable BT WITH(NOLOCK)
                    INNER JOIN tblBuildTableLayout BL WITH(NOLOCK) ON BL.BuildTableID = BT.ID
                    INNER JOIN (
	                    SELECT DISTINCT BuildTableID,DatabaseID
	                    FROM tblExternalBuildTableDatabase WITH(NOLOCK)
	                ) ExtDB ON BT.id = ExtDB.BuildTableID
	                LEFT OUTER JOIN tblBuildTableLayoutMailer BLM WITH(NOLOCK) on BLM.BuildTableLayoutID=BL.ID AND BLM.MailerID={mailerId}
                    LEFT JOIN tblBuildRAWFieldName BR WITH(NOLOCK) on BR.BuildTableLayoutID = BL.ID 
                    WHERE BL.iIsSelectable = 1 AND ExtDB.DatabaseId = {databaseId} ");
            lcSQL.Append(sListCondition2).Append(sListCondition3);
            lcSQL.Append(" AND ( BL.iIsMailerSpecific=0 or BLM.ID IS NOT NULL) ");
            lcSQL.Append(" ORDER BY BT .ID,BL.iDisplayOrder, BL.cFieldDescription ");
            var externalFieldDetails = new List<FieldData>();
            using (var command = _databaseHelper.CreateCommand(lcSQL.ToString(), CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        externalFieldDetails.Add(new FieldData
                        {
                            ID = Convert.ToInt32(dataReader["ID"]),
                            iDataLength = Convert.ToInt32(dataReader["iDataLength"]),
                            cFieldName = dataReader["cFieldName"].ToString(),
                            cTableName = dataReader["cTableName"].ToString(),
                            iShowTextBox = Convert.ToBoolean(dataReader["iShowTextBox"]),
                            iShowListBox = Convert.ToBoolean(dataReader["iShowListBox"]),
                            iFileOperations = Convert.ToBoolean(dataReader["iFileOperations"]),
                            iShowDefault = Convert.ToInt32(dataReader["iShowDefault"]),
                            cFieldType = dataReader["FieldDetail"].ToString(),
                            iDisplayOrder = Convert.ToInt32(dataReader["iDisplayOrder"]),
                            iIsListSpecific = Convert.ToBoolean(dataReader["iIsListSpecific"]),
                            cFieldDescription = dataReader["cFieldDescription"].ToString(),
                            iBTID = Convert.ToInt32(dataReader["ExBTID"]),
                            cDataType = dataReader["cDataType"].ToString()
                        });
                    }
                }
            }
            return externalFieldDetails;
        }

        public int GetSubSelBuildLolID(int iSubSelID, int buildId)
        {
            _databaseHelper.EnsureConnectionOpen();
            int result = 0;
            using (var command = _databaseHelper.CreateCommand($@"Select top 1  tblBuildLoL.ID as BuildLolID  from tblBuildLoL WITH(NOLOCK) 
                             inner join tblSubSelectList WITH(NOLOCK) on tblSubSelectList.MasterLOLID = tblBuildLoL.MasterLOLID                        
                             where SubSelectID={iSubSelID} and tblBuildLoL.BuildID = {buildId}", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }

        public int GetBuildLolID(int segmentId, int buildId)
        {
            _databaseHelper.EnsureConnectionOpen();
            int result = 0;
            using (var command = _databaseHelper.CreateCommand($@"SELECT top 1 tblBuildLoL.ID AS BuildLolID
                                                                 FROM tblBuildLoL
                                                                 WITH(NOLOCK)
                                                                 INNER JOIN tblSegmentList WITH(NOLOCK) ON tblSegmentList.MasterLOLID = tblBuildLoL.MasterLOLID
                                                                 WHERE tblBuildLoL.BuildID = {buildId}
                                                                 AND tblSegmentList.SegmentID = {segmentId}", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }

        public BuildDto GetBuildDetails(int OrderID)
        {
            _databaseHelper.EnsureConnectionOpen();
            var build = new BuildDto();
            using (var command = _databaseHelper.CreateCommand($@"SELECT tblBuild.cBuild , tblBuild.ID ,tblBuild.DatabaseID, D.DivisionID from tblBuild WITH (NOLOCK) INNER JOIN tblDatabase D WITH (NOLOCK) on tblBuild.DatabaseID= D.ID
                                INNER JOIN tblOrder WITH (NOLOCK) on tblBuild.ID = tblOrder.BuildID Where tblOrder.ID = {OrderID}", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        build.Id = Convert.ToInt32(dataReader["ID"]);
                        build.cBuild = dataReader["ID"].ToString();
                        build.DatabaseId = Convert.ToInt32(dataReader["DatabaseID"]);
                    }
                }
            }
            return build;
        }
    }

}
