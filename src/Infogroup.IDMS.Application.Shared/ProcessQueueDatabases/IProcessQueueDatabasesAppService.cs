using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ProcessQueueDatabases.Dtos;
using Infogroup.IDMS.Dto;
using System.Collections.Generic;

namespace Infogroup.IDMS.ProcessQueueDatabases
{
    public interface IProcessQueueDatabasesAppService : IApplicationService 
    {
		 Task CreateOrEdit(List<dropdownForProcessQueueDto> databases, int PQID);

		Task Delete(int Id);

		
    }
}