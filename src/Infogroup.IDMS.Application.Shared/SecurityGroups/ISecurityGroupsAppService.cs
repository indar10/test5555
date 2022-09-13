using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SecurityGroups.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.GroupBrokers.Dtos;

namespace Infogroup.IDMS.SecurityGroups
{
    public interface ISecurityGroupsAppService : IApplicationService 
    {
        PagedResultDto<SecurityGroupDto> GetAllSecurityGroups(GetAllSecurityGroupsInput input);

        Task CreateOrEdit(CreateOrEditSecurityGroupDto input);

        FileDto ExportToExcel(GetAllSecurityGroupsInput input);
    }
}