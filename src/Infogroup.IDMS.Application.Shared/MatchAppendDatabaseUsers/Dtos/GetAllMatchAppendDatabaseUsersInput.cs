using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.MatchAppendDatabaseUsers.Dtos
{
    public class GetAllMatchAppendDatabaseUsersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}