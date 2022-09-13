using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.Decoys.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Mailers.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Decoys
{
    public class DecoyRepository : IDMSRepositoryBase<Decoy, int>, IDecoyRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public DecoyRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public async Task<List<DecoyDto>> GetDecoyEntityListByMailer(int iMailerID)
        {
            var result = new List<DecoyDto>();
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand($@"
                                                Select D.ID, D.DatabaseID, D.cDecoyGroup, D.cDecoyType, 
                                                D.cKeyCode1, D.cFirstName, D.cLastName, D.cName, D.cAddress1, 
                                                D.cAddress2, D.cCity, D.cState, D.cZip, D.cZip4, D.cCompany, 
                                                D.cTitle, D.cEmail, D.cPhone, D.cFax, D.dCreatedDate, 
                                                D.cCreatedBy, D.dModifiedDate, D.cModifiedBy, D.MailerID 
                                                FROM  tblDecoy D 
                                                WHERE (cDecoyType = 'G' and D.DatabaseID in 
                                                (Select DatabaseID from tblMailer M where M.ID = {iMailerID})) 
                                                OR (cDecoyGroup='A' and MailerID = {iMailerID} )
                                                ", CommandType.Text))

            {                
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new DecoyDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            DatabaseId = Convert.ToInt32(dataReader["DatabaseID"]),
                            cDecoyGroup = dataReader["cDecoyGroup"].ToString(),
                            cDecoyType = dataReader["cDecoyType"].ToString(),
                            cKeyCode1 = dataReader["cKeyCode1"].ToString(),
                            cFirstName = dataReader["cFirstName"].ToString(),
                            cLastName = dataReader["cLastName"].ToString(),
                            cName = dataReader["cName"].ToString(),
                            cAddress1 = dataReader["cAddress1"].ToString(),
                            cAddress2 = dataReader["cAddress2"].ToString(),
                            cCity = dataReader["cCity"].ToString(),
                            cState = dataReader["cState"].ToString(),
                            cZip = dataReader["cZip"].ToString(),
                            cZip4 = dataReader["cZip4"].ToString(),
                            cCompany = dataReader["cCompany"].ToString(),
                            cTitle = dataReader["cTitle"].ToString(),
                            cEmail = dataReader["cEmail"].ToString(),
                            cPhone = dataReader["cPhone"].ToString(),
                            cFax = dataReader["cFax"].ToString(),
                            MailerID = Convert.ToInt32(dataReader["MailerID"])
                        });
                    }
                }              
            }
            return new List<DecoyDto>(result.ToList());
        }

        public PagedResultDto<MailerDto> GetAllDecoyMailers(int userId, GetAllDecoysInput input, string shortWhere)
        {
            var decoyMailers = new List<MailerDto>();
            var result = new PagedResultDto<MailerDto>();
            var whereQuery = string.Empty;
            _databaseHelper.EnsureConnectionOpen();

            var sorting = input.Sorting ?? "cCompany ASC";

            if (!string.IsNullOrWhiteSpace(shortWhere) && shortWhere.Length > 0)
                whereQuery = $"AND ({shortWhere})";
            else if(!string.IsNullOrWhiteSpace(input.Filter))
            {
                var isModelId = Validation.ValidationHelper.IsNumeric(input.Filter);
                if (isModelId)
                    whereQuery = $"AND M.ID IN ({input.Filter})";
                else
                    whereQuery = $@" AND (M.CCOMPANY LIKE '%'+@Filter+'%' OR M.CCODE LIKE '%'+@Filter+'%' )";
            }

            if (input.mailerId > 0)            
                whereQuery = $"{whereQuery} AND M.ID IN ({input.mailerId})";           

            using (var command = _databaseHelper.CreateCommand($@"WITH Decoy_CTE (ID, MailerID, cName, cZip, cAddress) AS (
	                                SELECT ID, MailerID, cName, cZip, Rtrim(Ltrim(CCITY + ',' + CSTATE + ' ' + CZIP)) + 
															( CASE ISNULL(cZip4, 0)
																WHEN 0 THEN '' ELSE (' - ' + cZip4) END +ISNULL(caddress1,'')+ISNULL(caddress2,'')
															) AS cAddress
	                                        FROM tblDecoy WITH (NOLOCK)
	                                        )
                                        SELECT Count(DISTINCT M.ID) 
                                        FROM TBLMAILER M  WITH(NOLOCK) 
                                        INNER JOIN tblGroupBroker GB  WITH(NOLOCK) ON GB.BrokerID = M.BrokerID
                                        INNER JOIN tblUserGroup UG  WITH(NOLOCK) ON UG.GroupID = GB.GroupID
                                        LEFT JOIN Decoy_CTE D ON M.ID = D.MailerID 
					Where M.DatabaseID = {input.SelectedDatabase.ToString()} And  UG.UserID = {userId} {whereQuery}", CommandType.Text))
            {
                command.Parameters.Add(new SqlParameter("@Filter", input.Filter ?? string.Empty));
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }

            using (var command = _databaseHelper.CreateCommand($@"
                                                WITH Decoy_CTE (ID, MailerID, cName, cZip, cAddress)
                                        AS (
	                                        SELECT ID, MailerID, cName, cZip, Rtrim(Ltrim(CCITY + ',' + CSTATE + ' ' + CZIP)) + (
			                                        CASE ISNULL(cZip4, 0)
				                                        WHEN 0
					                                        THEN ''
				                                        ELSE (' - ' + cZip4)
				                                        END
                                                    +ISNULL(caddress1,'')+ISNULL(caddress2,'')
			                                        ) AS cAddress
	                                        FROM tblDecoy WITH (NOLOCK)
	                                        )
                                        SELECT DISTINCT M.ID, M.CCODE, M.CCOMPANY, M.CCITY + ', ' + M.CSTATE + ' ' + M.CZIP AS Address, (
		                                        SELECT COUNT(*)
		                                        FROM TBLDecoy DCount
		                                        WHERE DCount.MailerID = M.ID
		                                        ) AS COUNTDECOY, M.IISACTIVE, M.cAddress1, M.cAddress2, M.cCity, M.cState, M.cZip, M.cPhone, M.cFax
                                        FROM TBLMAILER M  WITH(NOLOCK) 
                                        INNER JOIN tblGroupBroker GB  WITH(NOLOCK) ON GB.BrokerID = M.BrokerID
                                        INNER JOIN tblUserGroup UG  WITH(NOLOCK) ON UG.GroupID = GB.GroupID
                                        LEFT JOIN Decoy_CTE D ON M.ID = D.MailerID 
					Where M.DatabaseID = {input.SelectedDatabase.ToString()} And  UG.UserID = {userId} {whereQuery} Order By {sorting} 
                                                OFFSET {input.SkipCount} ROWS FETCH NEXT {input.MaxResultCount} ROWS ONLY ", CommandType.Text))

            {
                command.Parameters.Add(new SqlParameter("@Filter", input.Filter ?? string.Empty));
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        decoyMailers.Add(new MailerDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                           cCode = dataReader["CCODE"].ToString(),
                           cCompany = dataReader["CCOMPANY"].ToString(),
                           cAddress = dataReader["Address"].ToString(),
                           cAddress1 = dataReader["cAddress1"].ToString(),
                           cAddress2 = dataReader["cAddress2"].ToString(),
                           cState = dataReader["cState"].ToString(),
                           cZip = dataReader["cZip"].ToString(),
                           cPhone = dataReader["cPhone"].ToString(),
                           cCity = dataReader["cCity"].ToString(),
                           cFax = dataReader["cFax"].ToString(),
                            DecoysCount = Convert.ToInt32(dataReader["COUNTDECOY"]),
                           iIsActive = Convert.ToBoolean(dataReader["IISACTIVE"])
                        });
                    }
                    result.Items = decoyMailers;
                }
            }
            return result;
        }

        public PagedResultDto<DecoyDto> GetDecoysByMailer(GetAllDecoysInput input, string shortWhere)
        {
            var decoys = new List<DecoyDto>();
            var result = new PagedResultDto<DecoyDto>();
            var whereQuery = string.Empty;
            _databaseHelper.EnsureConnectionOpen();

            var sorting = input.Sorting ?? "ID ASC";

            if (!string.IsNullOrWhiteSpace(shortWhere) && shortWhere.Length > 0)
            {
                if (shortWhere.Contains("AND"))
                {
                    var splitedString = shortWhere.Split("AND");
                    foreach (var item in splitedString)
                    {
                        if (item.Trim().StartsWith("D.cName") || item.Trim().StartsWith("D.cZip") || item.Trim().StartsWith("D.cAddress"))                        
                            whereQuery = string.IsNullOrEmpty(whereQuery) ? $"{item}" : $" {whereQuery} AND {item}";                        
                    }
                }
                else
                {
                    if (shortWhere.Trim().StartsWith("D.cName") || shortWhere.Trim().StartsWith("D.cZip") || shortWhere.Trim().StartsWith("D.cAddress"))
                        whereQuery = shortWhere;                    
                }
            }

            if (!string.IsNullOrWhiteSpace(whereQuery))            
                whereQuery = $"Where {whereQuery}";
            

            using (var command = _databaseHelper.CreateCommand($@"WITH CTE_Decoy (ID, DatabaseID, MailerID, cDecoyType, cFirstName, cLastName, cName, cAddress1, cAddress2, cCity, cState, 
                cZip, cZip4, cCompany, cTitle, cEmail, cPhone, cFax, dCreatedDate, cCreatedBy, dModifiedDate, cModifiedBy, cKeyCode1, cDecoyGroup, cAddress)
                AS (
	                SELECT ID, DatabaseID, MailerID, cDecoyType, cFirstName, cLastName, cName, cAddress1, cAddress2, cCity, cState, cZip, cZip4, cCompany, cTitle, cEmail, cPhone, cFax, dCreatedDate, cCreatedBy, dModifiedDate, cModifiedBy, cKeyCode1, cDecoyGroup, Rtrim(Ltrim(CCITY + ',' + CSTATE + ' ' + CZIP)) + (
			        CASE ISNULL(cZip4, 0) WHEN 0 THEN '' ELSE (' - ' + cZip4) END + ISNULL(caddress1, '') + ISNULL(caddress2, '') ) AS cAddress
	                FROM tblDecoy WITH (NOLOCK)
	                )
                SELECT COUNT(DISTINCT D.ID)
                FROM CTE_Decoy D
                JOIN TBLMAILER M ON M.ID = D.MailerID AND D.MailerID = {input.mailerId} {whereQuery}", CommandType.Text))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }

            using (var command = _databaseHelper.CreateCommand($@"
                                                WITH CTE_Decoy (ID, DatabaseID, MailerID, cDecoyType, cFirstName, cLastName, cName, cAddress1, cAddress2, cCity, cState, 
                cZip, cZip4, cCompany, cTitle, cEmail, cPhone, cFax, dCreatedDate, cCreatedBy, dModifiedDate, cModifiedBy, cKeyCode1, cDecoyGroup, cAddress)
                AS (
	                SELECT ID, DatabaseID, MailerID, cDecoyType, cFirstName, cLastName, cName, cAddress1, cAddress2, cCity, cState, cZip, cZip4, cCompany, cTitle, cEmail, cPhone, cFax, dCreatedDate, cCreatedBy, dModifiedDate, cModifiedBy, cKeyCode1, cDecoyGroup, Rtrim(Ltrim(CCITY + ',' + CSTATE + ' ' + CZIP)) + (
			        CASE ISNULL(cZip4, 0) WHEN 0 THEN '' ELSE (' - ' + cZip4) END + ISNULL(caddress1, '') + ISNULL(caddress2, '') ) AS cAddress
	                FROM tblDecoy WITH (NOLOCK)
	                )
                SELECT D.ID, D.DatabaseID, D.cDecoyGroup, D.cDecoyType, D.cKeyCode1, D.cFirstName, D.cLastName, D.cName, D.cAddress1, D.cAddress2, 
                Rtrim(Ltrim(D.CCITY + ',' + D.CSTATE + ' ' + D.CZIP)) + ( CASE ISNULL(D.cZip4, 0) WHEN 0 THEN '' ELSE (' - ' + D.cZip4) END ) AS Address, 
		        M.CCOMPANY, D.cCity, D.cState, D.cZip, D.cZip4, D.cCompany, D.cTitle, D.cEmail, D.cPhone, D.cFax, 
		        D.dCreatedDate, D.cCreatedBy, D.dModifiedDate, D.cModifiedBy, D.MailerID
                FROM CTE_Decoy D
                JOIN TBLMAILER M ON M.ID = D.MailerID AND D.MailerID = {input.mailerId} {whereQuery} order by {sorting}
                OFFSET {input.SkipCount} ROWS FETCH NEXT {input.MaxResultCount} ROWS ONLY ", CommandType.Text))

            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        decoys.Add(new DecoyDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            cName = dataReader["cName"].ToString(),
                            cAddress1 = dataReader["cAddress1"].ToString(),
                            cAddress2 = dataReader["cAddress2"].ToString(),
                            cDecoyGroup = dataReader["cDecoyGroup"].ToString(),
                            cCompany = dataReader["cCompany"].ToString(),
                            cDecoyType = dataReader["cDecoyType"].ToString(),
                            cZip = dataReader["cZip"].ToString(),
                            cZip4 = dataReader["cZip4"].ToString(),
                            cPhone = dataReader["cPhone"].ToString(),
                            cFax = dataReader["cFax"].ToString(),
                            cEmail = dataReader["cEmail"].ToString(),
                            cKeyCode1 = dataReader["cKeyCode1"].ToString(),
                            cFirstName = dataReader["cFirstName"].ToString(),
                            cLastName = dataReader["cLastName"].ToString(),
                            cState = dataReader["cState"].ToString(),
                            cCity = dataReader["cCity"].ToString(),
                            cTitle = dataReader["cTitle"].ToString(),
                            cAddress = dataReader["Address"].ToString()
                        });
                    }
                    result.Items = decoys;
                }
            }
            return result;
        }

        public void CopyDecoy(int decoyId, string userName)
        {
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand($@"Insert into TblDecoy(
                        DatabaseID, MailerID, cDecoyType, cFirstName, cLastName, cName, cAddress1, cAddress2, cCity, cState, cZip, cZip4, cCompany, cTitle, cEmail, cPhone, cFax, dCreatedDate, cCreatedBy, dModifiedDate, cModifiedBy, cKeyCode1, cDecoyGroup
                        )
                        SELECT DatabaseID, MailerID, cDecoyType, cFirstName, cLastName, cName, cAddress1, cAddress2, cCity, cState, cZip, cZip4, cCompany, cTitle, cEmail, cPhone, cFax, GETDATE(), '{userName}', null, null, cKeyCode1, cDecoyGroup
                        FROM TblDecoy WITH(NOLOCK)
                        WHERE ID = {decoyId}", CommandType.Text))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
