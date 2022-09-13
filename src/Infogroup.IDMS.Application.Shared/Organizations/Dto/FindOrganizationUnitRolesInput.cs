using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Organizations.Dto
{
    public class FindOrganizationUnitRolesInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}