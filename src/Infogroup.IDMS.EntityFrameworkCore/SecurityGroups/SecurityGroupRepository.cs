using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.GroupBrokers.Dtos;
using Infogroup.IDMS.SecurityGroups.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.SecurityGroups
{
    public class SecurityGroupRepository : IDMSRepositoryBase<SecurityGroup, int>, ISecurityGroupRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;

        public SecurityGroupRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<SecurityGroupDto> GetAllSecurityGroupsList(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<SecurityGroupDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var securityGroupData = new List<SecurityGroupDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            securityGroupData.Add(new SecurityGroupDto
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                cGroupName = dataReader["cGroupName"].ToString(),
                                cGroupDescription = dataReader["cGroupDescription"].ToString(),
                                UserCount = Convert.ToInt32(dataReader["USERCOUNT"]),
                                iIsActive = Convert.ToBoolean(dataReader["iIsActive"])
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

        public List<string> GetAllGroupBroker(int GroupID)
        {
            try
            {
                var result = new List<string>();
                _databaseHelper.EnsureConnectionOpen();
                using (var command = _databaseHelper.CreateCommand($@"SELECT b.cCompany AS Company  FROM tblGroupBroker gb inner join tblBroker b on b.ID = gb.BrokerID where gb.GroupID = {GroupID}", CommandType.Text))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(dataReader["Company"].ToString().Trim());
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<string> GetAllGroupUsers(int GroupID)
        {
            try
            {
                var result = new List<string>();
                _databaseHelper.EnsureConnectionOpen();
                using (var command = _databaseHelper.CreateCommand($@"select u.cUserID + ' (' + u.cFirstName + ' ' + u.cLastName + ')' as [Name] from tblUser u inner join tblUserGroup ug on ug.UserID = u.ID and u.iIsActive = 1 and ug.GroupID = {GroupID}", CommandType.Text))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(dataReader["Name"].ToString().Trim());
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public PagedResultDto<UserCountDto> GetAllUserCountList(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<UserCountDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var securityGroupData = new List<UserCountDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            securityGroupData.Add(new UserCountDto
                            {
                                FirstName = dataReader["cFirstName"].ToString().Trim(),
                                LastName = dataReader["cLastName"].ToString().Trim(),
                                Email = dataReader["cEmail"].ToString().Trim()
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
    }
}
