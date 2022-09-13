using Infogroup.IDMS.ProcessQueues;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ProcessQueueDatabases.Exporting;
using Infogroup.IDMS.ProcessQueueDatabases.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Sessions;

namespace Infogroup.IDMS.ProcessQueueDatabases
{
	//[AbpAuthorize(AppPermissions.Pages_ProcessQueueDatabases)]
    public class ProcessQueueDatabasesAppService : IDMSAppServiceBase, IProcessQueueDatabasesAppService
    {
		 private readonly IRepository<ProcessQueueDatabase> _processQueueDatabaseRepository;
		 private readonly IProcessQueueDatabasesExcelExporter _processQueueDatabasesExcelExporter;
		 private readonly IRepository<ProcessQueue,int> _lookup_processQueueRepository;
        private readonly AppSession _mySession;
        private readonly IProcessQueueRepository _processQueueRepository;



        public ProcessQueueDatabasesAppService(IRepository<ProcessQueueDatabase> processQueueDatabaseRepository, 
              IProcessQueueDatabasesExcelExporter processQueueDatabasesExcelExporter ,
              IRepository<ProcessQueue, int> lookup_processQueueRepository, AppSession mySession,
              IProcessQueueRepository processQueueRepository) 
		  {
			_processQueueDatabaseRepository = processQueueDatabaseRepository;
			_processQueueDatabasesExcelExporter = processQueueDatabasesExcelExporter;
			_lookup_processQueueRepository = lookup_processQueueRepository;
            _mySession = mySession;
            _processQueueRepository = processQueueRepository;


          }

		 

		 public async Task CreateOrEdit(List<dropdownForProcessQueueDto> databases, int PQID)
         {
            for(int i=0;i< databases.Count; i++)
            {
                await Create(databases[i], PQID);
            }
		        	
         }

		 //[AbpAuthorize(AppPermissions.Pages_ProcessQueueDatabases_Create)]
		 private async Task Create(dropdownForProcessQueueDto databases,int PQID)        {

            // var ids = _processQueueDatabaseRepository.GetAll().Where(x => x.ProcessQueueId == PQID).ToList();
            if (databases.action == null)
            {
                databases.action = "1";
            }

            switch (databases.action)
            {
                case "0":
                    break;
                case "1":

                    var dwapObject = new CreateOrEditProcessQueueDatabaseDto
                    {
                        ProcessQueueId = PQID,
                        DatabaseId = databases.value,
                        cCreatedBy = _mySession.IDMSUserName,
                        dCreatedDate = DateTime.Now,

                    };
                    var dwapContact = ObjectMapper.Map<ProcessQueueDatabase>(dwapObject);
                    await _processQueueDatabaseRepository.InsertAsync(dwapContact);
                    CurrentUnitOfWork.SaveChanges();
                    break;
                case "3":
                    int primaryId = _processQueueRepository.GetID(databases.value, PQID);
                    _processQueueDatabaseRepository.Delete(primaryId);
                    break;
            }
                
               

            

         }
        public async Task Delete(int Id)
        {
           await _processQueueDatabaseRepository.DeleteAsync(Id);
        }
	}
}