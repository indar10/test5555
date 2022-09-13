using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infogroup.IDMS
{
    public static class CommonHelpers
    {
        private readonly static string stringConstant = "System.String";
        private readonly static List<string> propertyNames = new List<string> { "cModifiedBy", "cCreatedBy" };
      
        public static T ConvertNullStringToEmptyAndTrim<T>(T myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                var propertyType = pi.PropertyType.ToString();

                if (propertyType == stringConstant)
                {
                    var value= (string)pi.GetValue(myObject);
                    if (!propertyNames.Contains(pi.Name) && string.IsNullOrEmpty(value))
                        pi.SetValue(myObject, string.Empty);
                    else if (!string.IsNullOrEmpty(value))
                        pi.SetValue(myObject, value.Trim());
                }
            }
            return myObject;
        }
        public static string GetSplitCommaSeparatedString(string sText, int iNoOfSegments, bool isBulkUpload = false)
        {
            var sValues = new StringBuilder();
            var iOut = 0;
            var selectedIds = new List<string>();
            selectedIds.AddRange(sText.Split(','));
            var formatList = "{0},";

            foreach (string s in selectedIds)
            {
                if (s.Contains("-"))
                {
                    string[] sRange = s.Split('-');
                    //if more that 1 items are found in sColumnName (it means more that 1 search criteria)
                    if (sRange.Length == 2)
                    {
                        var iFromRange = 0;
                        var iToRange = 1;

                        if (int.TryParse(sRange[0], out iFromRange) && int.TryParse(sRange[1], out iToRange))
                        {
                            if (isBulkUpload && iToRange >= iNoOfSegments)
                                iToRange = iNoOfSegments;
                            
                            if (iFromRange <= iToRange && iToRange <= iNoOfSegments && iFromRange > 0 && iToRange > 0)
                            {
                                for (var iCounter = iFromRange; iCounter <= iToRange; iCounter++)
                                {
                                    sValues.Append(string.Format(formatList, iCounter.ToString()));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (int.TryParse(s, out iOut)) //If not a integer skip
                    {
                        if (iOut <= iNoOfSegments && iOut > 0)
                            sValues.Append(string.Format(formatList, s));
                    }
                }
            }
            if (!string.IsNullOrEmpty(sValues.ToString()))
            {
                sValues.Remove(sValues.Length - 1, 1);
                return sValues.ToString();
            }
            return string.Empty;
        }
    }
    public class ErrorMsg
    {
        public string LineNo { get; set; }
        public string Msg { get; set; }
        public ErrorMsg(string _lNo, string _msg)
        {
            if (!string.IsNullOrEmpty(_lNo))
            {
                var Ln = 0;
                Int32.TryParse(_lNo, out Ln);
                LineNo = (Ln + 2).ToString();
            }
            else
                LineNo = _lNo;
            
            Msg = _msg;
        }
    }
    public static class ErrorMessageFormatter
    {
        public static string GetNumberedMessage(List<ErrorMsg> input)
        {
            var msg = string.Empty;
            var builder = new StringBuilder();
            var counter = 1;
            foreach (ErrorMsg oMsg in input)
            {
                msg = builder.Append($"{counter}. ").Append(oMsg.Msg).Append(!string.IsNullOrEmpty(oMsg.LineNo) ? $" at line # { oMsg.LineNo}" : string.Empty).Append($".{Environment.NewLine}").ToString();
                counter++;
            }
            return msg;
        }
    }
}
