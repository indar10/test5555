using System.Collections.Generic;

namespace Infogroup.IDMS.ShortSearch
{
    public class SearchPart
    {
        public SearchPart(string shortcut, string values)
        {
            Shortcut = shortcut;
            Values = values;
        }

        public string Shortcut { get; set; }
        public string Values { get; set; }
    }

    public class ShortSearchField
    {
        public string Shortcut { get; set; }
        public string FieldNameText { get; set; }
        public string FieldNameNumber { get; set; }
        public string FieldDescription { get; set; }
    }

    public class ShortSearchItem
    {
        public string ID { get; set; }
        public List<ExampleText> ExampleHelpTexts { get; set; }
        public List<ShortSearchField> SearchFields { get; set; }
    }

    public class ExampleText
    {
        public string Text { get; set; }
    }

    public class HelpText
    {
        public string Header { get; set; }
        public List<string> Examples { get; set; }
    }
}
