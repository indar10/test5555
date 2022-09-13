using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.CampaignXTabReports.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.CampaignXTabReports
{
    public class CampaignXTabReportsRepository: IDMSRepositoryBase<CampaignXTabReport, int>, ICampaignXTabReportsRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public CampaignXTabReportsRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
           : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public List<GetCampaignXTabReportsListForView> GetAllCampaignXtabReports(int campaignID, int databaseID)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {         
                var lcSQL = string.Empty;
                lcSQL = $@"SELECT XTab.ID,ISNULL(L1.cDescription,'Field Not Found') as cXDesc,L2.cDescription as cYDesc, XTab.OrderID, XTab.cXField, XTab.cSegmentNumbers, XTab.cYField, XTab.iXTabBySegment, case  when iXTabBySegment = 1  then 'Segment' else 'Campaign' end as IsXTab,XTab.cType, case  when cType = 'N'  then 'Net' else 'Gross' end as cTypeName from tblOrderXTabReport XTab {sNoLock} 
             LEFT JOIN tblLookup L1 {sNoLock } ON RTRIM(XTab.cXField) = RTRIM(L1.cField) AND  (
            (L1.cLookupValue='XTABFIELDS' and L1.cCode='{databaseID}') 
             OR (L1.cLookupValue = 'XTABEXTERNAL' AND L1.cCode IN 
            (SELECT BuildTableID FROM tblExternalBuildTableDatabase {sNoLock}  WHERE DatabaseID = { databaseID})))
                 LEFT JOIN tblLookup L2 {sNoLock} ON RTRIM(XTab.cYField) = RTRIM(L2.cField) AND (
            (L2.cLookupValue='XTABFIELDS' and L2.cCode='{databaseID}')
                 OR (L2.cLookupValue = 'XTABEXTERNAL' AND L2.cCode IN 
            (SELECT BuildTableID FROM tblExternalBuildTableDatabase {sNoLock} WHERE DatabaseID = { databaseID})))
             WHERE OrderID = { campaignID }";

                using (var command = CreateCommand(lcSQL, CommandType.Text))
                {
                    var result = new List<GetCampaignXTabReportsListForView>();

                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new GetCampaignXTabReportsListForView
                            {
                                cXDesc = (dataReader["cXDesc"]).ToString().ToUpper().Equals("SPECIAL SIC") ? "Selected SIC":(dataReader["cXDesc"]).ToString(),
                                cYDesc = (dataReader["cYDesc"]).ToString(),
                                cXField = (dataReader["cXField"]).ToString(),
                                cYField = (dataReader["cYField"]).ToString(),
                                IsXTab = dataReader["IsXTab"].ToString(),
                                ID = Convert.ToInt32((dataReader["ID"])),
                                cType = (dataReader["cType"]).ToString(),
                                cSegmentNumbers = (dataReader["cSegmentNumbers"]).ToString(),
                                cTypeName = (dataReader["cTypeName"]).ToString(),
                                iXTabBySegment =Convert.ToBoolean(dataReader["iXTabBySegment"]),
                                Action = ActionType.None
                            });
                        }
                    }

                    return result;
                }
            }
            catch(Exception ex)
            {

                throw ex;
            }
        }
        public List<int> GetAllCampaignXtabReportIds(int campaignID)
        {
            _databaseHelper.EnsureConnectionOpen();
            List<int> result = new List<int>();
            try
            {
                var lcSQL = string.Empty;
                lcSQL = $@"SELECT ID from tblOrderXTabReport WHERE OrderID = { campaignID }";

                using (var command = CreateCommand(lcSQL, CommandType.Text))
                {
                   using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(Convert.ToInt32((dataReader["ID"])));  
                        }
                    }
                   return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private DbCommand CreateCommand(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            var command = Context.Database.GetDbConnection().CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = GetActiveTransaction();
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }
        
        private DbTransaction GetActiveTransaction()
        {
            return (DbTransaction)_transactionProvider.GetActiveTransaction(new ActiveTransactionProviderArgs
            {
                {"ContextType", typeof(IDMSDbContext) },
                {"MultiTenancySide", MultiTenancySide }
            });
        }

    }
}
