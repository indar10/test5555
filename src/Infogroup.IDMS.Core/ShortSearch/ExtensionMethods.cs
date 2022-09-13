using System;

namespace Infogroup.IDMS.ShortSearch.ExtensionMethods
{
    public static class ShortSearchExtensions
    {
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static bool IsDate(this string input)
        {
            DateTime dt;
            return (DateTime.TryParse(input, out dt));
        }

        public static bool IsNumeric(this string input)
        {
            Int32 intval;
            return (Int32.TryParse(input, out intval));
        }

        public static int Occurs(this string str, string val)
        {
            int occurrences = 0;
            int startingIndex = 0;

            while ((startingIndex = str.IndexOf(val, startingIndex)) >= 0)
            {
                ++occurrences;
                ++startingIndex;
            }

            return occurrences;
        }
    }
}
