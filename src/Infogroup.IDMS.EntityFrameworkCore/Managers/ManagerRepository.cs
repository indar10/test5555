using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Managers.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Managers
{
    public class ManagerRepository : IDMSRepositoryBase<Manager, int>, IManagerRepository
    {

        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public ManagerRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<ManagerDto> GetAllManagersList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<ManagerDto>();

            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var ownerData = new List<ManagerDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ownerData.Add(new ManagerDto
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

        public List<ContactAssignmentsDto> GetContactAssignmentsData(string query, List<SqlParameter> sqlParameters)
        {
            var result = new List<ContactAssignmentsDto>();

            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand(query, CommandType.Text, sqlParameters.ToArray()))
            {

                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new ContactAssignmentsDto
                        {
                            ListManager = Convert.ToString(dataReader["List Manager"]),
                            ContactName = Convert.ToString(dataReader["Name"]),
                            ContactId = Convert.ToInt32(dataReader["Id"])
                        });
                    }
                }
            }
            return new List<ContactAssignmentsDto>(result);
        }


    }
}
