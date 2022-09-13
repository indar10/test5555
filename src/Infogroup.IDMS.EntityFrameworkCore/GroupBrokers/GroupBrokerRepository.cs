using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.GroupBrokers;
using Infogroup.IDMS.GroupBrokers.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.GroupBrokers
{
    public class GroupBrokerRepository : IDMSRepositoryBase<GroupBroker, int>, IGroupBrokerRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;

        public GroupBrokerRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<GroupBrokerDto> GetAllGroupBrokersList(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<GroupBrokerDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var securityGroupData = new List<GroupBrokerDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            securityGroupData.Add(new GroupBrokerDto
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                cCode = dataReader["cCode"].ToString(),
                                cCompany = dataReader["cCompany"].ToString()
                            });
                        }
                    }
                    result.Items = securityGroupData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public PagedResultDto<GroupBrokerDto> GetAllBrokersList(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<GroupBrokerDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var securityGroupData = new List<GroupBrokerDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            securityGroupData.Add(new GroupBrokerDto
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                cCode = dataReader["cCode"].ToString(),
                                cCompany = dataReader["cCompany"].ToString(),
                                isSelected = Convert.ToBoolean(dataReader["CHECKED_STATUS"])
                            });
                        }
                    }
                    result.Items = securityGroupData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void DeleteBroker(int GroupID)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                using (var command = _databaseHelper.CreateCommand($@"DELETE FROM tblGroupBroker WHERE GroupID = {GroupID}", CommandType.Text))
                {
                    Convert.ToInt32(command.ExecuteNonQuery());
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

    }
}
