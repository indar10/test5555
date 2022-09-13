using System;
using System.Collections.Generic;
using System.Linq;
using Infogroup.IDMS.ShortSearch.ExtensionMethods;
using Newtonsoft.Json;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Lookups;

namespace Infogroup.IDMS.ShortSearch
{
    public class ShortSearch : IShortSearch
    {
        private readonly IRedisLookupCache _lookUpCache;
        private const string jsonLookupValue = "SHORTCUTSEARCH";
        public ShortSearch(IRedisLookupCache lookUpCache)
        {
          _lookUpCache = lookUpCache;
        }

        public HelpText GetSearchHelp(PageID pageID)
        {
            HelpText displayText = new HelpText();
            displayText.Header = "Search Options:";
            var searchItem = GetSearchSettings(pageID);
            displayText.Examples = searchItem.ExampleHelpTexts.Select(helpTxt => helpTxt.Text).ToList();
            if (displayText.Examples == null) displayText.Examples = new List<string>();
            foreach (var f in searchItem.SearchFields)
            {
                displayText.Examples.Add($"For {f.FieldDescription}, use {f.Shortcut}:Value\r\n");
            }
            return displayText;
        }

        private ShortSearchItem GetSearchSettings(PageID pageID)
        {
            //Fetch json from tblLookup where cLookupValue = "SHORTCUTSEARCH" AND cCode = pageID
            var cCode = ((int)pageID).ToString();
            var json = _lookUpCache.GetLookUpFields("SHORTCUTSEARCH", cCode).FirstOrDefault()?.mField;
            if (json == null) throw new Exception("Invalid JSON");
            return JsonConvert.DeserializeObject<ShortSearchItem>(json);
        }

        public string GetWhere(PageID pageID, string searchText)
        {
            var search = GetSearchSettings(pageID);
            var where = new List<string>();

            if (searchText != null && searchText.Contains(":"))
            {
                var parts = GetSearchParts(searchText);
                foreach (var p in parts)
                {
                    var searhField = search.SearchFields.Where(x => x.Shortcut.ToUpper() == p.Shortcut).ToList<ShortSearchField>().FirstOrDefault();
                    if (searhField != null)
                        where.Add(BuildWhere(searhField, p));
                    else
                        throw new Exception("Invalid Search Option");
                }
            }

            return string.Join(" AND ", where.Where(x => x.Length > 0));
        }

        private string BuildWhere(ShortSearchField searhField, SearchPart p)
        {
            string where = "";

            //Future Scenarios: Handle multiple CSV : 324,234,343
            //Future Scenarios: Handle Range : 10-20
            //Future Scenarios: Better Handling of Dates
            //Future Scenarios: >, <, >=, <= Operators
            if (p.Values.IsNumeric() && searhField.FieldNameNumber.Length > 0)                      //Handle Numeric
                where = $"{searhField.FieldNameNumber} = {p.Values}";
            else if (searhField.FieldNameText.Length > 0 && p.Values.IsDate())                      //Handle Date
                where = $"CAST({searhField.FieldNameText} as DATE) = '{p.Values}'";
            else if (searhField.FieldNameText.Length > 0 && !p.Values.IsDate())                     //Handle Alpha
                where = $"{searhField.FieldNameText} LIKE '%{p.Values}%'";

            //Invalid Input
            if (!p.Values.IsNumeric() && searhField.FieldNameText.Length == 0)
                throw new Exception($"Numeric Search Value is expected : {searhField.FieldDescription} - {p.Values}");

            return where;
        }

        private List<SearchPart> GetSearchParts(string searchText)
        {
            searchText = searchText.Replace("  ", " ").ToUpper();
            var parts = new List<SearchPart>();
            var searchValues = searchText.Split("AND");

            foreach (var p in searchValues)
            {
                if (p.Occurs(":") == 1)
                {
                    var items = p.Split(":");
                    parts.Add(new SearchPart(items[0].Trim(), items[1].Trim().Replace("'", "''")));

                }
                else
                    throw new Exception("Invalid Search Option");
            }

            return parts;
        }

    }

}
