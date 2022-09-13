using Abp.Application.Services.Dto;
using GraphQL.Types;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Types
{
    public class UserPagedResultGraphType : ObjectGraphType<PagedResultDto<UserDto>>
    {
        public UserPagedResultGraphType()
        {
            Field(x => x.TotalCount);
            Field(x => x.Items, type: typeof(ListGraphType<UserType>));
        }
    }
}