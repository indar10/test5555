using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.SubSelectSelections;
using Infogroup.IDMS.SubSelectSelections.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.SubSelectSelections
{
    public class SubSelectSelectionsRepository : IDMSRepositoryBase<SubSelectSelection, int>, ISubSelectSelectionsRepository
    {
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public SubSelectSelectionsRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _databaseHelper = databaseHelper;
        }

        public List<SubSelectSelectionsDTO> GetAllSubSelectSelections(int iSubSelectID, int iBuildLoLID)
        {
            var result = new List<SubSelectSelectionsDTO>();
            var lcSQL = string.Empty;
            var sCondition = string.Empty;
            var sIsRAWNotMapped = "  'Y' as iIsRAWNotMapped ";
            if (iBuildLoLID == 0)
            {  //Multple or 1 Lists
               //Text box as default
               //  sCondition = "'T' AS cValueMode";
                sCondition = " case when iIsListSpecific = 1 THEN  'T' else cValueMode END AS cValueMode ";
            }
            else
            {  //BuildLolID = single list
                //Show what ever was added
                sCondition = "cValueMode";
                sIsRAWNotMapped = @" CAST(CASE 
                  WHEN (BTL.iIsListSpecific = 1 AND BR.ID is null AND cFieldName LIKE 'RAW_Field%')
                     THEN 'N'
                  ELSE 'Y' END AS char(1)) as iIsRAWNotMapped ";
            }

            lcSQL = @" SELECT SS.ID,SubSelectID,'' as cSystemFileName,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,
                       CASE ISNULL(BR.ID,0) WHEN 0 THEN BTL.cFieldDescription  ELSE BR.cRAWFieldName END  AS Description ,
                       case cValueMode when 'F' then '' else cValues end as cValues," +
                        sCondition + @",cDescriptions,cValueOperator, BTL.ID as FieldID,BTL.iShowTextBox,BTL.iShowListBox,BTL.iFileOperations, BTL.iShowDefault, BTL.iIsListSpecific, '0' as IsZipRadius, BTL.cFieldType , " + sIsRAWNotMapped + " FROM TblSubSelectSelection" +
                    @" SS  inner join tblsubselect on tblsubselect.ID =  SS.SubSelectID
                       inner join tblSegment S on S.ID = tblsubselect.SegmentID
                       inner join tblOrder O on O.ID = S.OrderID 
                       inner join tblBuild B on B.ID = O.BuildID 
                       left join tblBuildTable BT on BT.BuildID = B.ID  and BT.cTableName = SS.cTableName
                       LEFT OUTER JOIN tblBuildTable BT2 ON BT2.ID IN (
		                        SELECT BT.ID
		                        FROM tblExternalBuildTableDatabase ExDB
		                        INNER JOIN tblBuild ON ExDB.DatabaseID = tblBuild.DatabaseID
		                        INNER JOIN tblBuildTable BT ON BT.ID = ExDB.BuildTableID
		                        WHERE tblBuild.ID = B.ID
			                        AND BT.cTableName = SS.cTableName
		                        )
                        LEFT JOIN tblBuildTableLayout BTL ON (
		                        BTL.BuildTableID = BT.ID
		                        OR BTL.BuildTableID = BT2.ID
		                        )
	                        AND (
		                        (
			                        SS.cQuestionFieldName = 'ZIP'
			                        AND (
				                        (
					                        len(CONVERT(VARCHAR(max), cValues)) > 0
					                        AND cValueMode = 'F'
					                        AND BTL.cFieldName = 'ZIPRADIUS'
					                        )
				                        OR (
					                        (
						                        len(CONVERT(VARCHAR(max), cValues)) = 0
						                        OR cValueMode <> 'F'
						                        )
					                        AND BTL.cFieldName = 'ZIP'
					                        )
				                        )
			                        )
		                        OR (
			                        SS.cQuestionFieldName <> 'ZIP'
			                        AND BTL.cFieldName = SS.cQuestionFieldName
			                        )
		                        )

                       Left Join tblBuildRAWFieldName BR on BR.BuildTableLayoutID = BTL.ID AND BR.BuildLoLID = " + iBuildLoLID +
                       " WHERE SS.SubSelectID =" + iSubSelectID + " Order By iGroupNumber, SS.ID ";

            lcSQL += " SELECT '' as id, iGroupNumber, COUNT(iGroupNumber) AS COUNT, cGrouping " +
            " FROM tblSubSelectSelection WHERE cValueMode in ('T','G') and SubSelectID = " + iSubSelectID +
            " GROUP BY iGroupNumber, cGrouping  union all " +
            " SELECT id, 999 as iGroupNumber, 1 as COUNT, cGrouping " +
            " FROM tblSubSelectSelection WHERE cValueMode in ('F') and SubSelectID = " + iSubSelectID +
            " ORDER BY iGroupNumber ";
            
            using (var command = _databaseHelper.CreateCommand(lcSQL.ToString(), CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var selection = new SubSelectSelectionsDTO
                        {
                            Id = dataReader["ID"] is DBNull ? 0 : Convert.ToInt32(dataReader["ID"]),
                            SubSelectId = dataReader["SubSelectID"] is DBNull ? 0 : Convert.ToInt32(dataReader["SubSelectID"]),
                            cQuestionFieldName = dataReader["cQuestionFieldName"] is DBNull ? string.Empty : dataReader["cQuestionFieldName"].ToString(),
                            cJoinOperator = dataReader["cJoinOperator"] is DBNull ? string.Empty : dataReader["cJoinOperator"].ToString(),
                            iGroupNumber = Convert.ToInt32(dataReader["iGroupNumber"]),
                            iGroupOrder = Convert.ToInt32(dataReader["iGroupOrder"]),
                            cGrouping = dataReader["cGrouping"] is DBNull ? string.Empty : dataReader["cGrouping"].ToString(),
                            cDescriptions = dataReader["cDescriptions"] is DBNull ? string.Empty : dataReader["cDescriptions"].ToString(),
                            cValueMode = dataReader["cValueMode"] is DBNull ? string.Empty : dataReader["cValueMode"].ToString(),
                            cValueOperator = dataReader["cValueOperator"] is DBNull ? string.Empty : dataReader["cValueOperator"].ToString(),
                            cValues = dataReader["cValues"] is DBNull ? string.Empty : dataReader["cValues"].ToString(),
                            cQuestionDescription = dataReader["cQuestionDescription"] is DBNull ? string.Empty : dataReader["cQuestionDescription"].ToString(),

                        };
                        result.Add(selection);
                    }
                }
            }
            return result;
        }        

        public void UpdateSubSelectSelection(int selectionId, string cGrouping)
        {
            _databaseHelper.EnsureConnectionOpen();
            var lcSQL = new StringBuilder();
            lcSQL.AppendLine($@"UPDATE tblSubSelectSelection SET cGrouping = '{cGrouping}'");
            lcSQL.AppendFormat($@" WHERE ID = {selectionId}");
            using (var command = _databaseHelper.CreateCommand(lcSQL.ToString(), CommandType.Text))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}

