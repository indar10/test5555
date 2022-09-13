using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Abp.Domain.Repositories;
using Abp.Authorization;
using Infogroup.IDMS.Shared.Dtos;
using Abp.UI;

namespace Infogroup.IDMS.IntentTopics
{
	[AbpAuthorize]
    public class IntentTopicsAppService : IDMSAppServiceBase, IIntentTopicsAppService
    {
		 private readonly IRepository<IntentTopic> _intentTopicRepository;		 

		  public IntentTopicsAppService(IRepository<IntentTopic> intentTopicRepository ) 
		  {
			_intentTopicRepository = intentTopicRepository;			
		  }

		 public List<DropdownOutputDto> GetAllIntentTopics()
         {					
            try
            {
                return _intentTopicRepository.GetAll()
                .Select(intentTopic => new DropdownOutputDto
                {
                    Value = intentTopic.cTopic,
                    Label = intentTopic.cTopic
                }).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
         }	 		 
    }
}