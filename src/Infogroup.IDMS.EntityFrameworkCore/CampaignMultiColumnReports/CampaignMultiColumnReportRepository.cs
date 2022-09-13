using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.CampaignMultiColumnReports;

namespace Infogroup.IDMS.CampaignMultiColumnReports
{
    public class CampaignMultiColumnReportRepository : IDMSRepositoryBase<CampaignMultiColumnReport, int>, ICampaignMultiColumnReportRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public CampaignMultiColumnReportRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
           : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

     

        public List<GetCampaignMultidimensionalReportForViewDto> GetAllCampaignMultiDimensionalReports(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {            

                using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
                {
                    var result = new List<GetCampaignMultidimensionalReportForViewDto>();

                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new GetCampaignMultidimensionalReportForViewDto
                            {
                                cFieldDescription = (dataReader["cDesc"]).ToString(),
                                cFieldName = (dataReader["cFields"]).ToString(),
                                cFields = (dataReader["cFields"]).ToString().Split(','),
                                IsMulti = dataReader["IsMCol"].ToString(),
                                ID = Convert.ToInt32((dataReader["ID"])),
                                cType = (dataReader["cType"]).ToString(),
                                cSegmentNumbers = (dataReader["cSegmentNumbers"]).ToString(),
                                cTypeName = (dataReader["cTypeName"]).ToString(),
                                IMultiBySegment = Convert.ToBoolean(dataReader["iMultiColBySegment"]),
                                Action = ActionType.None
                            });
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
    }
}
