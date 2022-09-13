using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using Abp.Application.Services.Dto;
using System.Data.SqlClient;
using Infogroup.IDMS.Brokers.Dtos;

namespace Infogroup.IDMS.Brokers
{
    public class BrokerRepository : IDMSRepositoryBase<Broker, int>, IBrokerRepository
    {

        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public BrokerRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<BrokersDto> GetAllBrokersList(string selectQuery, string countQuery, List<SqlParameter> sqlParameters, string companyOrCode)
        {
            _databaseHelper.EnsureConnectionOpen();
           
            var result = new PagedResultDto<BrokersDto>();
            sqlParameters.Add(new SqlParameter("@Code", $"%{companyOrCode}%"));
            sqlParameters.Add(new SqlParameter("@Company", $"%{companyOrCode}%"));
            using (var command = _databaseHelper.CreateCommand(countQuery, CommandType.Text, sqlParameters.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var brokerData = new List<BrokersDto>();            

           
            using (var command = _databaseHelper.CreateCommand(selectQuery, CommandType.Text, sqlParameters.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        brokerData.Add(new BrokersDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            cCompany = dataReader["CCOMPANY"].ToString(),
                            cCode = dataReader["CCODE"].ToString().Trim(),
                            cCity = dataReader["CCity"].ToString(),
                            cState = dataReader["CSTATE"].ToString().Trim(),
                            cAddress1 = dataReader["cAddress1"].ToString(),
                            cAddress2 = dataReader["cAddress2"].ToString(),
                            cPhone = dataReader["cPhone"].ToString(),
                            cFax = dataReader["cFax"].ToString(),
                            cZip = dataReader["cZip"].ToString(),
                            cAddress = dataReader["Address"] != null ? dataReader["Address"].ToString() : string.Empty,
                            ContactsCount = Convert.ToInt32(dataReader["COUNTCONTACT"])
                        });
                    }
                }
                result.Items = brokerData;
            }
            return result;
        }

        
    }
}
