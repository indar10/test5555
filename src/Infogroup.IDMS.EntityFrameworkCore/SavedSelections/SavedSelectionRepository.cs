using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.SavedSelections.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SavedSelections
{
    public class SavedSelectionRepository : IDMSRepositoryBase<SavedSelection, int>, ISavedSelectionRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        private const string sNoLock = " WITH (NOLOCK) ";
        public SavedSelectionRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public async Task<PagedResultDto<GetSavedSelectionForViewDto>> GetAllSavedSelection(int databaseID, int userID, GetAllSavedSelectionsInput input)
        {
            _databaseHelper.EnsureConnectionOpen();
            GetSavedSelectionForViewDto savedSelection;
            var result = new PagedResultDto<GetSavedSelectionForViewDto>();
            var items = new List<GetSavedSelectionForViewDto>();            
            var sorting = input.Sorting ?? "ID ASC";
            if (sorting.Contains("iIsDefault ASC"))
                sorting = sorting.Replace("iIsDefault", "bUserDefault ASC,iIsDefault");
            else if (sorting.Contains("iIsDefault DESC"))
                sorting = sorting.Replace("iIsDefault", "bUserDefault DESC,iIsDefault");
            var whereQuery = string.Empty;

            if (!string.IsNullOrEmpty(input.Filter))
            {
                var isSavedSelectionId = Validation.ValidationHelper.IsNumeric(input.Filter);
                whereQuery = isSavedSelectionId ? $" And SS.ID IN ({input.Filter})" : $" And ss.cDescription Like '%'+@Filter+'%'";
            }
            using (var command = _databaseHelper.CreateCommand($@"select sum(recordsCount) from (
                Select count(*) as recordsCount
                from tblSavedSelection SS  {sNoLock}INNER JOIN tblUserGroup UG on SS.GroupID = UG.GroupID  
                WHERE SS.iIsActive = 1 	And SS.DatabaseID = {databaseID} And UG.UserID = {userID} {whereQuery}
                UNION 
                Select count(*) as recordsCount
                from tblUserSavedSelection  SS {sNoLock} WHERE SS.iIsActive = 1 And SS.DatabaseID = {databaseID} And SS.UserID =  {userID} {whereQuery}
                ) recordsCount", CommandType.Text))
            {
                command.Parameters.Add(new SqlParameter("@Filter", input.Filter ?? string.Empty));
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }

            using (var command = _databaseHelper.CreateCommand($@" Select SS.ID,SS.cDescription,'' as cChannelType,'' as iIsDefault,cast(CASE when exists 
                ( Select * from tblSavedSelectionDetail  SSD  {sNoLock} where SSD.SavedSelectionID = SS.ID and cJoinOperator like 'OR' and  iIsActive = 1)
                then 1 else 0 end  AS bit) AS bIsOr, 'False' AS bUserDefault
                from tblSavedSelection  SS  {sNoLock} INNER JOIN tblUserGroup UG on SS.GroupID = UG.GroupID
                WHERE SS.iIsActive = 1 	And SS.DatabaseID = {databaseID} And UG.UserID = {userID} {whereQuery}
                UNION
                Select SS.ID,SS.cDescription,SS.cChannelType,SS.iIsDefault,cast(CASE when exists
                ( Select * from tblUserSavedSelectionDetail  SSD {sNoLock} where SSD.UserSavedSelectionID = SS.ID and cJoinOperator like 'OR' and  iIsActive = 1) then 1 else 0 end  AS bit) AS bIsOr, 'True' AS bUserDefault
                from tblUserSavedSelection   SS {sNoLock} WHERE SS.iIsActive = 1 And SS.DatabaseID = {databaseID} And SS.UserID =  {userID} {whereQuery}
                Order By {sorting} OFFSET {input.SkipCount} ROWS FETCH NEXT {input.MaxResultCount} ROWS ONLY ", CommandType.Text))
            {
                command.Parameters.Add(new SqlParameter("@Filter", input.Filter ?? string.Empty));
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        savedSelection = new GetSavedSelectionForViewDto
                        {
                            ID = Convert.ToInt32(dataReader["ID"]),
                            cDescription = dataReader["cDescription"].ToString(),
                            IsOR = Convert.ToBoolean(dataReader["bIsOR"]),
                            UserDefault = Convert.ToBoolean(dataReader["bUserDefault"]),
                            cChannelType = dataReader["cChannelType"].ToString(),
                            iIsDefault = Convert.ToBoolean(dataReader["iIsDefault"])
                        };
                        items.Add(savedSelection);
                    }
                }
                result.Items = items;
            }
            return result;
        }
    }
}
