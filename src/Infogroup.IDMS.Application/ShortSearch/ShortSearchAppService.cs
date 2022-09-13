
using System;
using Abp.Authorization;
using Abp.UI;
using Infogroup.IDMS.Common;

namespace Infogroup.IDMS.ShortSearch
{
    [AbpAuthorize]
    public class ShortSearchAppService : IDMSAppServiceBase
    {
          private readonly IShortSearch _shortSearch;
          public ShortSearchAppService(IShortSearch shortSearch) 
		  {
            _shortSearch = shortSearch;
          }

		 public HelpText GetSearchHelpText(PageID pageID)
         {
           try
           {
                return _shortSearch.GetSearchHelp(pageID);
           }

            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

    }
}