using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Organizations.Dto
{
    public class FindOrganizationUnitUsersInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}
