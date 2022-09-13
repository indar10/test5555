using System.Text.RegularExpressions;
using Abp.Extensions;

namespace Infogroup.IDMS.Validation
{
    public static class ValidationHelper
    {
        public const string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string NumericRegex = @"^\d+(\s*\,\s*\d+)*$";
        public const string NumericRange = @"^\d+\s*-\s*\d+$";

        public static bool IsEmail(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }

            var regex = new Regex(EmailRegex);
            return regex.IsMatch(value);
        }

        public static bool IsNumeric(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }
            var regex = new Regex(NumericRegex);
            return regex.IsMatch(value);
        }
        public static bool IsNumericRange(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }
            var regex = new Regex(NumericRange);
            return regex.IsMatch(value);
        }
    }
}
