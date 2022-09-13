using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.SavedSelectionDetails;
using Infogroup.IDMS.SavedSelectionDetails.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SavedSelectionsDetails
{
    public class SavedSelectionDetailRepository : IDMSRepositoryBase<SavedSelectionDetail, int>, ISavedSelectionDetailRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        private const string sNoLock = " WITH (NOLOCK) ";
        public SavedSelectionDetailRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public async Task<List<GetSavedSelectionDetailForViewDto>> GetAllSavedSelectionDetail(GetAllSavedSelectionDetailsInput input)
        {
            _databaseHelper.EnsureConnectionOpen();
            GetSavedSelectionDetailForViewDto savedSelectionDetail;
            var items = new List<GetSavedSelectionDetailForViewDto>();

            var sqlQuery = string.Empty;
            if (input.userDefault) {
                sqlQuery = $@"SELECT SSD.ID, UserSavedSelectionID, cQuestionFieldName, cQuestionDescription, cJoinOperator, iGroupNumber, 
                iGroupOrder, cGrouping, cast(cValues as varchar) cValues, cValueMode, cast(cDescriptions as varchar) cDescriptions, cValueOperator, iIsActive,
                tblBuildTableLayout.cFieldDescription, tblBuildTableLayout.ID as FieldID, tblBuildTableLayout.iIsListSpecific
                FROM tblUserSavedSelectionDetail SSD {sNoLock}
                INNER JOIN tblBuildTableLayout {sNoLock} ON tblBuildTableLayout.cFieldName = SSD.cQuestionFieldName
                INNER JOIN tblBuildTable BT {sNoLock} on BT.ID = tblBuildTableLayout.BuildTableID AND SSD.cTableName = BT.cTableName
                AND SSD.iIsActive = 1 AND SSD.UserSavedSelectionID = { input.savedSelectionID }
                UNION
                select SSD.ID, UserSavedSelectionID, cQuestionFieldName, cQuestionDescription, cJoinOperator, iGroupNumber, 
                iGroupOrder, cGrouping, cast(cValues as varchar) cValues, cValueMode, cast(cDescriptions as varchar) cDescriptions, cValueOperator, iIsActive,
                btl.cFieldDescription, btl.ID as FieldID, btl.iIsListSpecific
                from tblBuildTableLayout btl {sNoLock}
                INNER JOIN tblBuildTable bt {sNoLock} on btl.BuildTableID = bt.ID
                INNER JOIN tblBuild b {sNoLock} on bt.BuildID = b.id
                INNER JOIN tblUserSavedSelectionDetail SSD {sNoLock} on SSD.cQuestionFieldName = btl.cFieldName
                WHERE b.ID = { input.segmentID }
                and SSD.iIsActive = 1 AND SSD.UserSavedSelectionID = { input.savedSelectionID }
                AND SSD.cTableName is null
                ORDER BY iGroupNumber, iGroupOrder ASC";
            }
            else
            {
                sqlQuery = $@"SELECT SSD.ID, SavedSelectionID, cQuestionFieldName, cQuestionDescription, cJoinOperator, iGroupNumber, 
                iGroupOrder, cGrouping, cast(cValues as varchar(max)) cValues, cValueMode, cast(cDescriptions as varchar(max)) cDescriptions, cValueOperator, iIsActive,
                tblBuildTableLayout.cFieldDescription, tblBuildTableLayout.ID as FieldID, tblBuildTableLayout.iIsListSpecific
                FROM tblSavedSelectionDetail SSD {sNoLock}
                INNER JOIN tblBuildTableLayout {sNoLock} ON tblBuildTableLayout.cFieldName = SSD.cQuestionFieldName
                INNER JOIN tblBuildTable BT {sNoLock} on BT.ID = tblBuildTableLayout.BuildTableID AND SSD.cTableName = BT.cTableName
                AND SSD.iIsActive = 1 AND SSD.SavedSelectionID = {input.savedSelectionID }
                UNION
                select SSD.ID, SavedSelectionID, cQuestionFieldName, cQuestionDescription, cJoinOperator, iGroupNumber, 
                iGroupOrder, cGrouping, cast(cValues as varchar(max)) cValues, cValueMode, cast(cDescriptions as varchar(max)) cDescriptions, cValueOperator, iIsActive,
                btl.cFieldDescription, btl.ID as FieldID, btl.iIsListSpecific
                from tblBuildTableLayout btl {sNoLock}
                INNER JOIN tblBuildTable bt {sNoLock} on btl.BuildTableID = bt.ID
                INNER JOIN tblBuild b {sNoLock} on bt.BuildID = b.id
                INNER JOIN tblSavedSelectionDetail SSD {sNoLock} on SSD.cQuestionFieldName = btl.cFieldName
                WHERE b.ID = {input.segmentID } and SSD.iIsActive = 1 AND SSD.SavedSelectionID = {input.savedSelectionID } AND SSD.cTableName is null
                ORDER BY iGroupNumber, iGroupOrder ASC";
            }

            using (var command = _databaseHelper.CreateCommand(sqlQuery, CommandType.Text))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        savedSelectionDetail = new GetSavedSelectionDetailForViewDto
                        {
                            iGroupNumber = Convert.ToInt32(dataReader["iGroupNumber"]),
                            cFieldDescription = dataReader["cFieldDescription"].ToString(),
                            cJoinOperator = dataReader["cJoinOperator"].ToString(),
                            cValueOperator = dataReader["cValueOperator"].ToString(),
                            cValues = dataReader["cValues"].ToString()
                        };
                        items.Add(savedSelectionDetail);
                    }
                }                
            }
            return items;
        }
    }
}
