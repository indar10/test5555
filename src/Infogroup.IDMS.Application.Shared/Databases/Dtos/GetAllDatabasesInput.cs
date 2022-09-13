using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Databases.Dtos
{
    public class GetAllDatabasesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

        public int UserID { get; set; }
    }
}