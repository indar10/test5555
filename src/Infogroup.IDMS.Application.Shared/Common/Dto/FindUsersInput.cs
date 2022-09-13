using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Common.Dto
{
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? TenantId { get; set; }
    }
}