using Infogroup.IDMS.Common;

namespace Infogroup.IDMS.ShortSearch
{
    public interface IShortSearch
    {
        HelpText GetSearchHelp(PageID pageID);
        string GetWhere(PageID pageID, string searchText);
    }
}
