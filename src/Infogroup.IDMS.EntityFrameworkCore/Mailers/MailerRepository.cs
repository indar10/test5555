using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using Abp.Application.Services.Dto;
using System.Data.SqlClient;
using Infogroup.IDMS.Mailers.Dtos;

namespace Infogroup.IDMS.Mailers
{
    public class MailerRepository : IDMSRepositoryBase<Mailer, int>, IMailerRepository
    {

        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public MailerRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<MailerDto> GetAllMailersList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<MailerDto>();

            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var mailerData = new List<MailerDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        mailerData.Add(new MailerDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            DatabaseId = Convert.ToInt32(dataReader["DatabaseID"]),
                            cCompany = dataReader["CCOMPANY"].ToString(),
                            broker = dataReader["BROKER"].ToString(),
                            cCode = dataReader["CCODE"].ToString().Trim(),
                            cCity = dataReader["CCity"].ToString(),
                            cState = dataReader["CSTATE"].ToString().Trim(),
                            cAddress1 = dataReader["cAddress1"].ToString(),
                            cAddress2 = dataReader["cAddress2"].ToString(),
                            cPhone = dataReader["cPhone"].ToString(),
                            cFax = dataReader["cFax"].ToString(),
                            cZip = dataReader["cZip"].ToString(),
                            cAddress = dataReader["Address"] != null ? dataReader["Address"].ToString() : string.Empty,
                            iIsActive = Convert.ToBoolean(dataReader["iIsActive"]),
                            ContactsCount = Convert.ToInt32(dataReader["contactsCount"])
                        });
                    }
                }
                result.Items = mailerData;
            }
            return result;
        }
    }
}