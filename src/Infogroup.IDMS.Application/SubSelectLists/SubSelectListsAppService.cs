using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.SubSelectLists.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SubSelectLists
{
    [AbpAuthorize(AppPermissions.Pages_SubSelects)]
    public class SubSelectListsAppService : IDMSAppServiceBase, ISubSelectListsAppService
    {
		 private readonly IRepository<SubSelectList> _subSelectListRepository;

        public SubSelectListsAppService(IRepository<SubSelectList> subSelectListRepository) 
		{
			_subSelectListRepository = subSelectListRepository;
		}        
            public async Task<List<GetSubSelectSourcesForView>> GetSubSelectSourcesForEdit(int subSelectId)
            {
                try
                {                    
                    var subSelectSource = await _subSelectListRepository.GetAll().Where(x => x.SubSelectId == subSelectId)
                        .Select(selection => new GetSubSelectSourcesForView { Id = selection.Id, MasterLOLID = selection.MasterLOLID })
                        .ToListAsync();
                    return subSelectSource;
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }

        
    }
}