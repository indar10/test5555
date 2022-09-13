using Abp.Domain.Repositories;
using Abp.Authorization;
using Infogroup.IDMS.IDMSUsers;
using System.Collections.Generic;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Sessions;
using System;
using Abp.UI;

namespace Infogroup.IDMS.Divisions
{
	[AbpAuthorize]
    public class DivisionsAppService : IDMSAppServiceBase, IDivisionsAppService
    {
		 private readonly IRepository<Division> _divisionRepository;
         private readonly IRedisIDMSUserCache _userCache;
         private readonly AppSession _mySession;



        public DivisionsAppService(IRepository<Division> divisionRepository, 
            IRedisIDMSUserCache userCache ,
            AppSession mySession) 
		{
			_divisionRepository = divisionRepository;
            _userCache = userCache;
            _mySession = mySession;
        }

        public List<DropdownOutputDto> GetDivisionDropdownsForUser()
        {
            try
            {
                return _userCache.GetDropdownOptions(_mySession.IDMSUserId, UserDropdown.Divisions);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
    }
}