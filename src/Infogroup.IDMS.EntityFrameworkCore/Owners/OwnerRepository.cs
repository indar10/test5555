using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Owners.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using Abp.Application.Services.Dto;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Owners
{
    public class OwnerRepository : IDMSRepositoryBase<Owner, int>, IOwnerRepository
    {

        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public OwnerRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<OwnerDto> GetAllOwnersList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<OwnerDto>();
          

            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var ownerData = new List<OwnerDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ownerData.Add(new OwnerDto
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
                            ContactsCount = Convert.ToInt32(dataReader["contactsCount"])
                        });
                    }
                }
                result.Items = ownerData;
            }
            return result;
        }
    }
}