using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Databases.Dtos
{
    public class GetDatabaseAccessReportDto
    {

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string UserID { get; set; }

		public string Email { get; set; }
    }
}