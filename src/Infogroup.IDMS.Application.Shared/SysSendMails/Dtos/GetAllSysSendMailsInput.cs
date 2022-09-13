using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SysSendMails.Dtos
{
    public class GetAllSysSendMailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}