using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.GroupBrokers.Dtos;
using Infogroup.IDMS.SecurityGroups.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.SecurityGroups
{
    public interface ISecurityGroupRepository: IRepository<SecurityGroup,int>
    {
        PagedResultDto<SecurityGroupDto> GetAllSecurityGroupsList(Tuple<string, string, List<SqlParameter>> query);

        List<string> GetAllGroupBroker(int GroupID);

        List<string> GetAllGroupUsers(int GroupID);

        PagedResultDto<UserCountDto> GetAllUserCountList(Tuple<string, string, List<SqlParameter>> query);
    }
}
