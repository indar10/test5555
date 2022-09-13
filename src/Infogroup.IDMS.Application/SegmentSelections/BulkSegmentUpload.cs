using System;
using System.Data;
using Infogroup.IDMS.Segments.Dtos;
using System.Linq;
using Syncfusion.XlsIO;
using Abp.UI;
using System.Collections.Generic;
using Infogroup.IDMS.Segments;
using Infogroup.IDMS.SegmentSelections.Dtos;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Abp.AspNetZeroCore.Net;
using Infogroup.IDMS.Dto;
using System.Threading.Tasks;
using Infogroup.IDMS.Constants;
using System.Collections;
using Infogroup.IDMS.OrderStatuss;

namespace Infogroup.IDMS.SegmentSelections
{
    public partial class SegmentSelectionsAppService : IDMSAppServiceBase, ISegmentSelectionsAppService
    {    
        private ExcelEngine excelEngine;
        List<int> segList = new List<int>();
        List<SegmentSelectionDto> lstSegDetails = new List<SegmentSelectionDto>();
        string Message = string.Empty;
        private IWorkbook workBook;
        private readonly List<ErrorMsg> ErrMsg = new List<ErrorMsg>();
        private SegmentSelectionDto selDetails;
        private int databaseId;
        private int campaignId;
        private bool isGroupByKeyCode1;
        private bool isPopulateKeyCode1;
        private bool isDefaultFormat;
        private readonly List<string> AllOperatorList = new List<string> { Global.LikeWithUpper, Global.NotLike, Global.Between, Global.NotBetween, Global.IN, Global.NotIN, Global.Greater, Global.Less, Global.GreaterEqual, Global.LessEqual, Global.GreaterThanOperator, Global.LessThanOperator, Global.GreaterThanEqualToOperator, Global.LessThanEqualToOperator };
        private readonly string[] InvalidOperatorsList = new string[] { Global.LikeWithUpper, Global.NotLike, Global.Between, Global.NotBetween, Global.GreaterThanOperator, Global.LessThanOperator, Global.GreaterThanEqualToOperator, Global.LessThanEqualToOperator, Global.Greater, Global.Less, Global.GreaterEqual, Global.LessEqual };

        public async Task<bool> SaveBulkSegmentFileData(BulkSegmentDto bulkSegmentData)
        {
            var isSuccess = false;
            try
            {
                if (bulkSegmentData != null)
                {
                    databaseId = bulkSegmentData.DatabaseId;
                    campaignId = bulkSegmentData.CampaignId;
                    isGroupByKeyCode1 = bulkSegmentData.IsGroupByKeyCode1;
                    isPopulateKeyCode1 = bulkSegmentData.IsPopulateKeycode;
                    isDefaultFormat = bulkSegmentData.IsDefaultFormat;
                    if (bulkSegmentData.IsDefaultFormat)
                        await SaveDefaultFormat(bulkSegmentData.Path);
                    else
                        await SaveRadioStationFormat(bulkSegmentData.Path);

                    if (ErrMsg.Count() > 0 && !string.IsNullOrEmpty(GetConsolidatedErrorMsg()))
                    {
                        Message = GetConsolidatedErrorMsg();
                        throw new UserFriendlyException(Message);
                    }
                    else
                    {
                        isSuccess = true;
                        await _orderStatusManager.UpdateOrderStatus(bulkSegmentData.CampaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);

            }
            finally
            {
                if (ErrMsg.Count > 0 && segList.Count > 0)
                    _customSegmentSelectionRepository.DeleteRecords(segList, campaignId);
            }
            return isSuccess;
        }

        #region Default Format
        private async Task<string> SaveDefaultFormat(string cPath)
        {
            try
            { 
                excelEngine = new ExcelEngine();
                var application = excelEngine.Excel;
                var inputStream = new FileStream(cPath, FileMode.Open);
                workBook = application.Workbooks.Open(inputStream);
                                
                var minReqColumns = new ArrayList { "Segment No", "Description", "Field Name", "Values" };
                var providedColumns = new ArrayList();
                var sheet = workBook.Worksheets[0];
                var dt = new DataTable();

                var columnLength = sheet.Columns.Length;
                var rowLength = sheet.Rows.Length;
                for (int i = 0; i < sheet.Columns.Length; i++)
                {
                    if (sheet.Columns[i].DisplayText.ToString().Equals(string.Empty))
                        columnLength--;
                }               
                for (int i = 1; i < sheet.Rows.Length; i++)
                {
                    var empty = true;
                    for (int j = 0; j < columnLength; j++)
                    {
                        if (!sheet.Rows[i].Columns[j].DisplayText.ToString().Equals(string.Empty))
                        {
                            empty = false;
                            break;
                        }
                    }
                    if (empty)
                        rowLength--;
                }
                
                for (int j = 0; j < columnLength; j++)
                {
                    var columnHeader = sheet.Rows[0].Columns[j].Text;
                    providedColumns.Add(columnHeader);
                }                
                var matchFound = 0;
                var matchEqualFound = 0;
                const int minReqColumnsCount = 4;
                var incorrectColumnMatch = string.Empty;
                var reqColumnNotFound = string.Empty;
                var listAllColumns = new ArrayList { "Segment No", "Description", "Required Quantity", "Keycode1", "Keycode2", "Double Multibuyer", "MaxPer Group", "Title Suppression", "Fixed Title", "Auto Suppress", "Net Group", "Field Name", "Values", "Group Number", "Value Operator", "Table Name", "Random Radius Nth", "Field Name1", "Field Name2", "Field Name3", "Field Name4", "Values1", "Values2", "Values3", "Values4", "Value Operator1", "Value Operator2", "Value Operator3", "Value Operator4" };
                var listReqColumnsSeq = new ArrayList();

                if (providedColumns.Count == 0 || providedColumns.Contains(null))
                {
                    Message = L("CoulmnHeaderMsg");
                    ErrMsg.Add(new ErrorMsg(Message.ToString(), Message));
                    return Message;
                }
                
                for (int i = 0; i < minReqColumns.Count; i++)
                {
                    var found = false;
                    for (int j = 0; j < providedColumns.Count; j++)
                    {
                        if (providedColumns[j].ToString() != null)
                            if (minReqColumns[i].ToString().ToLower().Trim().Equals(providedColumns[j].ToString().ToLower().Trim()))
                            {
                                listReqColumnsSeq.Add(j);
                                matchFound++;
                                found = true;
                            }
                    }
                    if (!found)
                    {
                        if (reqColumnNotFound != string.Empty)
                            reqColumnNotFound = $"{reqColumnNotFound}, {minReqColumns[i].ToString()}";
                        else
                            reqColumnNotFound = minReqColumns[i].ToString();
                    }
                }
                if (matchFound != minReqColumnsCount)
                {
                    Message = L("RequiredColumnMsg", reqColumnNotFound);
                    ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                    return Message;
                }
                for (int i = 0; i < providedColumns.Count; i++)
                {
                    var isEqual = false;
                    for (int j = 0; j < listAllColumns.Count; j++)
                    {
                        if (providedColumns[i].ToString().ToLower().Trim().Equals(listAllColumns[j].ToString().ToLower().Trim()))
                        {
                            matchEqualFound++;
                            isEqual = true;
                        }
                    }
                    if (!isEqual)
                    {
                        if (incorrectColumnMatch == string.Empty)
                            incorrectColumnMatch = providedColumns[i].ToString();
                        else
                            incorrectColumnMatch = $"{incorrectColumnMatch}{Global.Comma}{providedColumns[i]}";
                    }
                }                
                if (matchEqualFound != providedColumns.Count)
                {
                    Message = L("ProvidedColumnMsg", incorrectColumnMatch);
                    ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                    return Message;
                }
                var excelColumns = providedColumns.Cast<string>().ToList().ConvertAll(d => d.ToUpper());
                var IsNotFound = false;
                if ((excelColumns.Contains("FIELD NAME1") || excelColumns.Contains("VALUES1")) && (!(excelColumns.Contains("FIELD NAME1") && excelColumns.Contains("VALUES1"))))
                {
                    IsNotFound = true;
                    ErrMsg.Add(new ErrorMsg(string.Empty, L("FieldNamesMsg", 1)));
                }
                if ((excelColumns.Contains("FIELD NAME2") || excelColumns.Contains("VALUES2")) && (!(excelColumns.Contains("FIELD NAME2") && excelColumns.Contains("VALUES2"))))
                {
                    IsNotFound = true;
                    ErrMsg.Add(new ErrorMsg(string.Empty, L("FieldNamesMsg", 2)));
                }
                if ((excelColumns.Contains("FIELD NAME3") || excelColumns.Contains("VALUES3")) && (!(excelColumns.Contains("FIELD NAME3") && excelColumns.Contains("VALUES3"))))
                {
                    IsNotFound = true;
                    ErrMsg.Add(new ErrorMsg(string.Empty, L("FieldNamesMsg", 3)));
                }
                if ((excelColumns.Contains("FIELD NAME4") || excelColumns.Contains("VALUES4")) && (!(excelColumns.Contains("FIELD NAME4") && excelColumns.Contains("VALUES4"))))
                {
                    IsNotFound = true;
                    ErrMsg.Add(new ErrorMsg(string.Empty, L("FieldNamesMsg", 4)));
                }
                if (IsNotFound)
                    return Message;

                var columnOrder = 0;                
                for (int i = 0; i < providedColumns.Count; i++)
                {
                    for (int j = 0; j < listAllColumns.Count; j++)
                    {
                        if (providedColumns[i].ToString().ToLower().Trim().Equals(listAllColumns[j].ToString().ToLower().Trim()))
                        {
                            var columnStringType = Type.GetType("System.String");
                            var columnInt16Type = Type.GetType("System.Int16");
                            var column = listAllColumns[j].ToString().ToUpper();

                            var dColumn = new DataColumn { ColumnName = column };
                            switch (column.Trim().ToUpper())
                            {
                                case "SEGMENT NO":
                                case "DESCRIPTION":
                                case "KEYCODE1":
                                    dColumn.DataType = columnStringType;
                                    dColumn.MaxLength = 50;
                                    break;
                                case "REQUIRED QUANTITY":
                                case "DOUBLE MULTIBUYER":
                                case "MAXPER GROUP":
                                case "FIXED TITLE":
                                case "NET GROUP":
                                case "FIELD NAME":
                                case "FIELD NAME1":
                                case "FIELD NAME2":
                                case "FIELD NAME3":
                                case "FIELD NAME4":
                                case "VALUES":
                                case "VALUES1":
                                case "VALUES2":
                                case "VALUES3":
                                case "VALUES4":
                                case "GROUP NUMBER":
                                case "VALUE OPERATOR":
                                case "VALUE OPERATOR1":
                                case "VALUE OPERATOR2":
                                case "VALUE OPERATOR3":
                                case "VALUE OPERATOR4":
                                case "TABLE NAME":
                                    dColumn.DataType = columnStringType;
                                    break;
                                case "KEYCODE2":
                                    dColumn.DataType = columnStringType;
                                    dColumn.MaxLength = 15;
                                    break;
                                case "TITLE SUPPRESSION":
                                    dColumn.DataType = columnStringType;
                                    dColumn.MaxLength = 1;
                                    break;
                                case "AUTO SUPPRESS":
                                case "RANDOM RADIUS NTH":
                                    dColumn.DataType = columnInt16Type;
                                    break;
                                default:
                                    break;
                            }
                            dt.Columns.Add(dColumn);
                            columnOrder++;
                        }
                    }
                }                
                if (rowLength == 1)
                {
                    Message = L("EmptyMsg");
                    ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                    return Message;
                }
                var segdt = sheet.ExportDataTable(sheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                var segNo = (from table in segdt.AsEnumerable()
                             select table.Field<string>("Segment No")).Distinct().ToList();

                if (segNo.Count() - 1 > 10000)
                {
                    Message = L("10kSegmentMsg");
                    ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                    return Message;
                }
                for (int i = 1; i < rowLength && ErrMsg.Count < 5; i++)
                {
                    var drField = dt.NewRow();

                    for (int j = 0; j < providedColumns.Count && ErrMsg.Count < 5; j++)
                    {
                        var minReqColumnMatch = false;
                        for (int mCount = 0; mCount < minReqColumns.Count && ErrMsg.Count < 5; mCount++)
                        {
                            if (minReqColumns[mCount].ToString().ToLower().Equals(dt.Columns[j].ColumnName.ToString().ToLower()))
                            {
                                if (string.IsNullOrEmpty(sheet.Rows[i].Columns[j].Value.ToString()))
                                {
                                    if (minReqColumns[mCount].ToString() == "Segment No" ||
                                       minReqColumns[mCount].ToString() == "Description" ||
                                       minReqColumns[mCount].ToString() == "Field Name" ||
                                       minReqColumns[mCount].ToString() == "Values")
                                    {
                                        Message = L("ValueEmptyMsg", minReqColumns[mCount]);
                                        ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                    }
                                }
                                else if ("Segment No" == minReqColumns[mCount].ToString())
                                {
                                    var num = 0;
                                    if (!int.TryParse(sheet.Rows[i].Columns[j].Value.ToString(), out num))
                                    {
                                        Message = L("IntValueMsg", minReqColumns[mCount]);
                                        ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                    }
                                }
                                else if ("Description" == minReqColumns[mCount].ToString() && sheet.Rows[i].Columns[j].Value.Count() > 50)
                                {
                                    Message = L("CharExceedMsg", "Segment Description", 50);
                                    ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                }
                                drField[j] = sheet.Rows[i].Columns[j].Value;
                                minReqColumnMatch = true;
                                break;
                            }
                        }
                        if (!minReqColumnMatch)
                        {
                            var valueCol = sheet.Rows[i].Columns[j].Value.ToString();
                            var colName = dt.Columns[j].ColumnName.ToString().ToUpper();
                            if (valueCol == string.Empty)
                            {

                                if (colName == "FIELD NAME1" || colName == "FIELD NAME2" || colName == "FIELD NAME3" || colName == "FIELD NAME4" || colName == "VALUES1" || colName == "VALUES2" || colName == "VALUES3" || colName == "VALUES4")
                                    drField[j] = string.Empty;

                                if (colName == "REQUIRED QUANTITY" || colName == "DOUBLE MULTIBUYER")
                                    drField[j] = string.Empty;

                                else if (colName == "MAXPER GROUP")
                                    drField[j] = "00";

                                else if (colName == "KEYCODE1" || colName == "KEYCODE2" || colName == "TITLE SUPPRESSION" || colName == "FIXED TITLE")
                                    drField[j] = string.Empty;

                                else if (colName == "NET GROUP" || colName == "GROUP NUMBER")
                                    drField[j] = 1;
                                else if (colName == "AUTO SUPPRESS")
                                    drField[j] = true;

                                else if (colName == "RANDOM RADIUS NTH")
                                    drField[j] = false;

                                else if (colName == "VALUE OPERATOR" || colName == "VALUE OPERATOR1" || colName == "VALUE OPERATOR2" || colName == "VALUE OPERATOR3" || colName == "VALUE OPERATOR4")
                                    drField[j] = string.Empty;

                                else
                                    drField[j] = sheet.Rows[i].Columns[j].Value;

                            }
                            else if (colName == "NET GROUP" || colName == "GROUP NUMBER" || colName == "DOUBLE MULTIBUYER" || colName == "REQUIRED QUANTITY")
                            {
                                var num = 0;
                                var numericResult = int.TryParse(valueCol, out num);
                                if (!numericResult)
                                {
                                    Message = L("IntValueMsg", colName.ToLower());
                                    ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                }
                                else
                                    drField[j] = sheet.Rows[i].Columns[j].Value;

                            }
                            else if (colName == "AUTO SUPPRESS" || colName == "RANDOM RADIUS NTH")
                            {
                                var num = 0;
                                if (!int.TryParse(sheet.Rows[i].Columns[j].Value.ToString(), out num))
                                {
                                    Message = L("Int1ValueMsg", colName.ToLower());
                                    ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                }
                                var value = Convert.ToInt32(sheet.Rows[i].Columns[j].Value);
                                if (value > 2 || value < 0)
                                {
                                    Message = L("Int1ValueMsg", colName.ToLower());
                                    ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                }
                                else
                                    drField[j] = sheet.Rows[i].Columns[j].Value;

                            }
                            else if (colName == "MAXPER GROUP")
                            {
                                if (Convert.ToString(valueCol).PadLeft(2, '0') != string.Empty)
                                {
                                    var listOfcCode = _lookupCache.GetLookUpFields("MAXPER", Convert.ToString(valueCol).PadLeft(2, '0')).Select(t => t.cCode).ToArray();
                                    if (listOfcCode.Length == 1)
                                    {
                                        var maxPervalue = listOfcCode[0].PadLeft(2, '0');
                                        if (Convert.ToInt32(maxPervalue) < 00 || Convert.ToInt32(maxPervalue) > 10)
                                        {
                                            Message = L("InvalidMaxPerGroupMsg");
                                            ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                        }
                                        else
                                            drField[j] = maxPervalue;
                                    }
                                    else
                                    {
                                        Message = L("InvalidMaxPerGroupMsg");
                                        ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                    }
                                }
                                else
                                {
                                    Message = L("InvalidMaxPerGroupMsg");
                                    ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                }
                            }
                            else
                            {
                                if (colName == "KEYCODE1" && valueCol.Count() > 50)
                                {
                                    Message = L("CharExceedMsg", "KeyCode1", 50);
                                    ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                }
                                else if (colName == "KEYCODE2" && valueCol.Count() > 15)
                                {
                                    Message = L("CharExceedMsg", "KeyCode2", 15);
                                    ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                                }
                                drField[j] = sheet.Rows[i].Columns[j].Value.Trim();
                            }
                        }
                    }
                    if (dt.Columns.Contains("Required Quantity"))
                    {
                        int.TryParse(Convert.ToString(drField["REQUIRED QUANTITY"]), out int iMaxVal);
                        if (!string.IsNullOrEmpty(drField["REQUIRED QUANTITY"].ToString().Trim()))
                        {
                            if (Convert.ToString(drField["REQUIRED QUANTITY"]).Trim() != iMaxVal.ToString() ||
                             Convert.ToInt32(drField["REQUIRED QUANTITY"]) < 0)
                            {
                                Message = L("InvalidReqRangeMsg", int.MaxValue);
                                ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                            }
                        }
                    }
                    if (dt.Columns.Contains("Double Multibuyer"))
                    {
                        if (!string.IsNullOrEmpty(drField["DOUBLE MULTIBUYER"].ToString()))
                        {
                            if (Convert.ToInt32(drField["DOUBLE MULTIBUYER"]) < 2 || Convert.ToInt32(drField["DOUBLE MULTIBUYER"]) > 10)
                            {
                                Message = L("InvalidDoubleMultiBuyerMsg");
                                ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                            }
                        }
                    }

                    string _titleSupp, _FixedTitle;
                    _titleSupp = _FixedTitle = string.Empty;

                    if (dt.Columns.Contains("Title Suppression"))
                        _titleSupp = Convert.ToString(drField["TITLE SUPPRESSION"]).ToUpper();

                    if (dt.Columns.Contains("Fixed Title"))
                        _FixedTitle = Convert.ToString(drField["FIXED TITLE"]).ToUpper();

                    ValidateAndAssignOrderOutputFormat(_titleSupp, _FixedTitle, (i - 1).ToString());

                    var obj = new Regex("^[1-9]|0[1-9]$");
                    if (dt.Columns.Contains("Net Group"))
                    {
                        if (!obj.IsMatch(Convert.ToString(drField["NET GROUP"])))
                        {
                            Message = L("InvalidNetMsg");
                            ErrMsg.Add(new ErrorMsg((i - 1).ToString(), Message));
                        }
                    }
                    dt.Rows.Add(drField);
                }
                
                if (!isGroupByKeyCode1)
                {
                    ValidateSegmentNo(dt);
                    if (ErrMsg.Count >= 5)
                        return GetConsolidatedErrorMsg();

                    else if (ErrMsg.Count > 0)
                        return GetConsolidatedErrorMsg();

                    if (!ValidateSegmentSelectionValidations(dt) || ErrMsg.Count > 0)
                        Message = GetConsolidatedErrorMsg();

                }                
                if (isGroupByKeyCode1)
                {  
                    var workbook = excelEngine.Excel.Workbooks.Create(2);
                    var fieldName1 = new List<string>();
                    var fieldName2 = new List<string>();
                    var fieldName3 = new List<string>();
                    var fieldName4 = new List<string>();
                    var results = (IEnumerable<DataRow>)null;

                    var columns = dt.Columns.Cast<DataColumn>().Where(c => c.ColumnName != "Keycode1" && c.ColumnName != "Segment No").ToArray();

                    var qpOfKeycodeAndSeg = from row in dt.AsEnumerable()
                                            group row by new { SegmentNo = row.Field<string>("Segment No"), Keycode1 = row.Field<string>("Keycode1") } into grp
                                            select grp.ToList();

                    var qpOfSegment = from row in dt.AsEnumerable()
                                      group row by new { SegmentNo = row.Field<string>("Segment No") } into grpSeg
                                      select grpSeg.ToList();
                    foreach (var item in qpOfSegment)
                    {
                        var fieldName = item.Select(r => r.Field<string>("Field Name", DataRowVersion.Current).ToLower()).Distinct().ToList();
                        results = from DataRow myRow in dt.Rows where myRow["Field Name"].ToString().ToUpper() == "GEORADIUS" select myRow;
                        if (results != null && results.Count() > 0)
                            break;
                        if (dt.Columns.Contains("Field Name1"))
                        {
                            fieldName1 = item.Select(r => r.Field<string>("Field Name1", DataRowVersion.Current).ToLower()).Distinct().ToList();
                            results = from DataRow myRow in dt.Rows where myRow["Field Name1"].ToString().ToUpper() == "GEORADIUS" select myRow;
                            if (results != null && results.Count() > 0)
                                break;
                        }
                        if (dt.Columns.Contains("Field Name2"))
                        {
                            fieldName2 = item.Select(r => r.Field<string>("Field Name2", DataRowVersion.Current).ToLower()).Distinct().ToList();
                            results = from DataRow myRow in dt.Rows where myRow["Field Name2"].ToString().ToUpper() == "GEORADIUS" select myRow;
                            if (results != null && results.Count() > 0)
                                break;
                        }
                        if (dt.Columns.Contains("Field Name3"))
                        {
                            fieldName3 = item.Select(r => r.Field<string>("Field Name3", DataRowVersion.Current).ToLower()).Distinct().ToList();
                            results = from DataRow myRow in dt.Rows where myRow["Field Name3"].ToString().ToUpper() == "GEORADIUS" select myRow;
                            if (results != null && results.Count() > 0)
                                break;
                        }
                        if (dt.Columns.Contains("Field Name4"))
                        {
                            fieldName4 = item.Select(r => r.Field<string>("Field Name4", DataRowVersion.Current).ToLower()).Distinct().ToList();
                            results = from DataRow myRow in dt.Rows where myRow["Field Name4"].ToString().ToUpper() == "GEORADIUS" select myRow;
                            if (results != null && results.Count() > 0)
                                break;
                        }
                        var keycode1 = item.Select(r => r.Field<string>("KeyCode1", DataRowVersion.Current)).Distinct().ToList();
                        if (fieldName.Count() > 1 && item.First().Field<string>("Segment No") != string.Empty)
                        {
                            Message = L("FieldGroupByKeycode1Msg", string.Empty, item.First().Field<string>("Segment No"));
                            ErrMsg.Add(new ErrorMsg(string.Empty, Message));

                        }
                        if (fieldName1.Count() > 1 && item.First().Field<string>("Segment No") != string.Empty)
                        {
                            Message = L("FieldGroupByKeycode1Msg", 1, item.First().Field<string>("Segment No"));
                            ErrMsg.Add(new ErrorMsg(string.Empty, Message));

                        }
                        if (fieldName2.Count() > 1 && item.First().Field<string>("Segment No") != string.Empty)
                        {
                            Message = L("FieldGroupByKeycode1Msg", 2, item.First().Field<string>("Segment No"));
                            ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                        }
                        if (fieldName3.Count() > 1 && item.First().Field<string>("Segment No") != string.Empty)
                        {
                            Message = L("FieldGroupByKeycode1Msg", 3, item.First().Field<string>("Segment No"));
                            ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                        }
                        if (fieldName4.Count() > 1 && item.First().Field<string>("Segment No") != string.Empty)
                        {
                            Message = L("FieldGroupByKeycode1Msg", 4, item.First().Field<string>("Segment No"));
                            ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                        }
                        if (keycode1.Contains(string.Empty) && item.First().Field<string>("Segment No") != string.Empty)
                        {
                            Message = L("GroupByKeyCode1SegmentNo1Msg", item.First().Field<string>("Segment No"));
                            ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                        }
                        if (keycode1.Count() > 1 && item.First().Field<string>("Segment No") != string.Empty)
                        {
                            Message = L("OneKeyCode1Msg", item.First().Field<string>("Segment No"));
                            ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                        }
                    }
                    if (results != null && results.Count() > 0)
                    {
                        Message = L("GeoRadiusKeycode1Msg");
                        ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                    }
                    if (ErrMsg.Count > 0)
                    {
                        Message = GetConsolidatedErrorMsg();
                        return Message;
                    }
                    var rdTable = dt.Clone();
                    foreach (var group in qpOfKeycodeAndSeg)
                    {
                        var nRow = rdTable.NewRow();
                        nRow["Keycode1"] = group.First().Field<string>("Keycode1");
                        nRow["Segment No"] = group.First().Field<string>("Segment No");
                        foreach (var column in columns)
                        {
                            if (column.ColumnName != "VALUES" && column.ColumnName != "VALUES1" && column.ColumnName != "VALUES2" && column.ColumnName != "VALUES3" && column.ColumnName != "VALUES4")
                                nRow[column.ColumnName] = group.First()[column.ColumnName];
                            else
                            {
                                var joinValues = string.Join(",", group.Select(r => r.Field<string>(column, DataRowVersion.Current).ToUpper()).Distinct().Distinct().ToArray());
                                var arrString = joinValues.Split(',');
                                var finaString = arrString.Distinct().ToList();
                                nRow[column.ColumnName] = string.Join(",", new List<string>(finaString).ToArray());
                            }

                        }
                        rdTable.Rows.Add(nRow);
                    }
                    ValidateSegmentNo(rdTable);
                    if (ErrMsg.Count > 0)
                    {
                        Message = GetConsolidatedErrorMsg();
                        return Message;
                    }
                    if (!ValidateSegmentSelectionValidations(rdTable) || ErrMsg.Count > 0)
                        Message = GetConsolidatedErrorMsg();
                    else
                    {
                        await SaveSegmentRules(rdTable);
                    }
                }
                else
                {
                    if (ErrMsg.Count > 0)
                        Message = GetConsolidatedErrorMsg();
                    else
                    {
                        await SaveSegmentRules(dt);                        
                    }
                }
                
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ErrMsg.Add(new ErrorMsg(string.Empty, Message));
                return Message;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close();
                    excelEngine.Dispose();
                }
            }

            return Message;
        }
        private void ValidateAndAssignOrderOutputFormat(string OutputOption, string OutputOptionText, string lineNo)
        {
            if (!string.IsNullOrEmpty(OutputOption))
            {
                var isValidSupression = _lookupCache.GetLookUpFields("ORDEROUTPUTFORMAT").Where(t => string.Compare(t.cCode, OutputOption, true) == 0).Select(t => t).ToArray();

                if (isValidSupression.Count() <= 0)
                    ErrMsg.Add(new ErrorMsg(lineNo, L("InvalidTitleSupressionMsg")));

                switch (OutputOption.Trim().ToUpper())
                {
                    case "N":
                    case "F":
                    case "O":
                        if (string.IsNullOrEmpty(OutputOptionText) || OutputOptionText.Replace(Global.Space, string.Empty).Length <= 0)
                            ErrMsg.Add(new ErrorMsg(lineNo, L("InvalidFixedTitleMsg")));
                        else
                        {
                            if (OutputOptionText.Length > 50)
                                ErrMsg.Add(new ErrorMsg(lineNo, L("CharFiexedTitleMsg")));
                        }
                        break;
                    case "B":
                    case "T":
                        break;
                    default:
                        break;

                }
            }
        }
        private void ValidateSegmentNo(DataTable dt)
        {
            var dtAll = new DataTable();
            dtAll = dt.Copy();
            var MaxExistingSegNo = 0;
            var listOfiDeupIds = _segmentRepository.GetAll().Where(t => !t.iIsOrderLevel && t.OrderId.Equals(campaignId)).Distinct().Select(t => t.iDedupeOrderSpecified).ToList();
            if (listOfiDeupIds != null && listOfiDeupIds.Count > 0)
            {
                DataRow dr = null;
                foreach (var ExistingDr in listOfiDeupIds)
                {
                    dr = dtAll.NewRow();
                    dr["Segment No"] = ExistingDr.ToString();
                    dtAll.Rows.Add(dr);
                }
                Int32.TryParse(listOfiDeupIds.Max().ToString(), out MaxExistingSegNo);
            }
            var Duplicates = dtAll.AsEnumerable().GroupBy(r => r["Segment No"]).Where(gr => gr.Count() > 1);

            if (Duplicates.Count() > 0 && !isGroupByKeyCode1)
                ErrMsg.Add(new ErrorMsg(string.Empty, L("SegmentUniqueMsg", Duplicates.FirstOrDefault().Key)));
            else
            {
                int currentSegNo, NextSegNo;
                Int32.TryParse(dt.Rows[0]["Segment No"].ToString(), out currentSegNo);
                if (currentSegNo != (MaxExistingSegNo + 1))
                {
                    if (isGroupByKeyCode1 && Duplicates.Count() > 0)
                        ErrMsg.Add(new ErrorMsg(string.Empty, L("SegmentAndIndexUniqueMsg", Duplicates.FirstOrDefault().Key, MaxExistingSegNo + 1)));
                    else
                        ErrMsg.Add(new ErrorMsg(string.Empty, L("SegmentNoShouldStartMsg", MaxExistingSegNo + 1)));
                    return;
                }
                for (int counter = 1; counter < dt.Rows.Count; counter++)
                {
                    Int32.TryParse(dt.Rows[counter]["Segment No"].ToString(), out NextSegNo);
                    if (NextSegNo == 0)
                    {
                        ErrMsg.Add(new ErrorMsg((counter + 1).ToString(), L("SegmentNoValidMsg")));
                        break;
                    }
                    if ((currentSegNo + 1) != NextSegNo)
                    {
                        ErrMsg.Add(new ErrorMsg(counter.ToString(), L("SegmentNoIncrementByOneMsg")));
                        break;
                    }
                    currentSegNo = NextSegNo;
                }
            }
        }
        #endregion

        #region RadioTv/Station Format
        private async Task SaveRadioStationFormat(string cPath)
        {
            try
            {
                excelEngine = new ExcelEngine();
                var application = excelEngine.Excel;
                var inputStream = new FileStream(cPath, FileMode.Open);
                workBook = application.Workbooks.Open(inputStream);
                var sheet = workBook.Worksheets[0];
                sheet.UsedRangeIncludesFormatting = false;
                var radiodt = sheet.ExportDataTable(sheet.UsedRange, ExcelExportDataTableOptions.ColumnNames | ExcelExportDataTableOptions.ComputedFormulaValues);

                ValidateRadioStationFile(radiodt);

                var groupedRadioStations = radiodt.AsEnumerable()
                                          .GroupBy(x => new
                                          {
                                              radioStationName = x.Field<string>(radiodt.Columns[0].ColumnName).Trim().ToUpper(),
                                          })
                                       .Select(x => new
                                       {
                                           values = string.Join(",", x.Select(z => z.Field<string>(radiodt.Columns[1].ColumnName))),
                                           stationName = x.Key.radioStationName
                                       }).ToList();
                var rdTable = new DataTable();
                rdTable.Columns.Add("Segment No");
                rdTable.Columns.Add("Description");
                rdTable.Columns.Add("Field Name");
                rdTable.Columns.Add("Values");
                rdTable.Columns.Add("KeyCode1");
                var segmentNo = _segmentRepository.GetAll().Count(t => !t.iIsOrderLevel && t.OrderId.Equals(campaignId)) > 0 ? _segmentRepository.GetAll().Where(t => !t.iIsOrderLevel && t.OrderId.Equals(campaignId)).Distinct().Max(t => t.iDedupeOrderSpecified) + 1 : 1;
                foreach (var item in groupedRadioStations)
                {
                    if (radiodt.Columns[1].ColumnName.ToUpper() == "GEORADIUS")
                        throw new UserFriendlyException(L("ValidGeoRadioMsg"));
                    else
                    {
                        var nRow = rdTable.NewRow();
                        nRow["Segment No"] = segmentNo;
                        nRow["Description"] = item.stationName;
                        nRow["Field Name"] = radiodt.Columns[1].ColumnName;
                        nRow["Values"] = item.values;
                        if (isPopulateKeyCode1)
                            nRow["KeyCode1"] = item.stationName;
                        else
                            nRow["KeyCode1"] = string.Empty;
                        segmentNo++;
                        rdTable.Rows.Add(nRow);
                    }
                }
                if (rdTable.Rows.Count > 5000)
                    throw new UserFriendlyException(L("ExcelFile5KMsg"));

                if (ValidateSegmentSelectionValidations(rdTable) && ErrMsg.Count == 0)
                    await SaveSegmentRules(rdTable);
            }
            catch (Exception ex)
            {
                ErrMsg.Add(new ErrorMsg(string.Empty, ex.Message));
            }
        }

        private void FillSegmentFieldDetails(string fieldName, string value, string valueOperator, ListSegmentFieldDetails lists)
        {
            if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(value))
            {
                lists.listFieldName.Add(fieldName);
                lists.listValues.Add(value);
                lists.listValueOperator.Add(valueOperator);
            }
        }
        private string GetConsolidatedErrorMsg()
        {
            var msg = string.Empty;
            var builder = new StringBuilder();
            var counter = 1;
            foreach (ErrorMsg oMsg in ErrMsg)
            {
                msg = builder.Append($"{counter}. ").Append(oMsg.Msg).Append(!string.IsNullOrEmpty(oMsg.LineNo) ? $" at line # { oMsg.LineNo}" : string.Empty).Append($".{Environment.NewLine}").ToString();
                counter++;
            }
            return msg;
        }
        #endregion

        #region Validations
        private bool ValidateGeoRadiusByMatchLevel(AddressInputDto input, string lineNo, out string zipradius, out string description, out string latlongValue)
        {
            var matchLevel = string.Empty;
            var flag = true;
            var matchLevels = new List<string>();
            zipradius = string.Empty;
            latlongValue = string.Empty;
            description = string.Empty;
            var radius = string.Empty;
            radius = input.AddressFilter.Split(':')[1];
            input.AddressFilter = input.AddressFilter.Split(':')[0];

            var endpointaddress = _appConfiguration["Services:Uri"];
            var service = new IDMSCommonService.IDMSIQServiceClient(endpointaddress);
            var response = service.VerifyGeoRadiusAsync(input.AddressFilter, input.DatabaseId, input.MainTableName);

            if (response != null)
            {
                var matchlevelValues = response.Result.VerifyGeoRadiusResult.OrderBy(t => t).ToArray();
                foreach (string item in matchlevelValues)
                {
                    if (!string.IsNullOrEmpty(item))
                        matchLevels.Add(item.Trim().Split(':')[0]);
                }
                foreach (string item in matchlevelValues)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        matchLevel = item.Trim().Split(':').ToArray()[0];

                        if (matchLevel.Equals("0"))
                        {
                            ErrMsg.Add(new ErrorMsg(lineNo.ToString(), $"{item.Split(':')[1]}"));
                            flag = false;
                        }
                        else if (matchLevel.Equals("1"))
                        {
                            description = $"{item.Split(':')[1]}:{radius}";
                            latlongValue = $"{item.Split(':')[2]}|{item.Split(':')[3]}:{radius}";
                            flag = true;
                        }
                        else if (matchLevel.Equals("3") && !matchLevels.Contains("1"))
                        {
                            var splitedvalues = item.Split(':');
                            var zip = splitedvalues[splitedvalues.Length - 1];
                            zipradius = $"{zip}-{radius}";
                            description = $"{item.Split(':')[1]}:{radius}";
                            flag = true;
                        }
                        else if (matchLevel.Equals("2") && !matchLevels.Contains("1") && !matchLevels.Contains("3"))
                        {
                            ErrMsg.Add(new ErrorMsg(lineNo.ToString(), L("NoMatchesFoundForGeoRadius")));
                            flag = false;
                        }
                    }
                }
            }

            return flag;
        }
        private void ValidateRadioStationFile(DataTable dt)
        {
            if (dt.Columns.Count != 2)
                throw new UserFriendlyException(L("ValidColumnRadioMsg"));

            var emptyIndexes = dt.AsEnumerable()
                  .Select((row, index) => new { Station = row[dt.Columns[0].ColumnName], Index = index + 2, fieldValue = row[dt.Columns[1].ColumnName] })
                  .Where(r => r.Station.Equals(DBNull.Value) || r.fieldValue.Equals(DBNull.Value)).Select(r => r.Index)
                  .ToList();

            if (emptyIndexes.Count > 0)
                throw new UserFriendlyException(L("ValidEmptyValuesRadioMsg", string.Join(",", emptyIndexes.ToArray())));

            var listMoreThan50Chars = dt.AsEnumerable()
                  .Select((row, index) => new { Station = row[dt.Columns[0].ColumnName], Index = index + 2, fieldValue = row[dt.Columns[1].ColumnName] })
                  .Where(t => t.Station.ToString().Length > 50).Select(r => r.Index)
                  .ToList();

            if (listMoreThan50Chars.Count > 0)
                throw new UserFriendlyException(L("ValidStationMsg", string.Join(",", listMoreThan50Chars.ToArray())));
        }
        private bool IsValidOperator(string[] InvalidOperators, string CurrentOperator)
        {
            var inAllOperator = AllOperatorList.Contains(CurrentOperator);
            var notInAllOperator = !InvalidOperators.Contains(CurrentOperator);
            return inAllOperator && notInAllOperator;
        }
        private bool ValidateText(string Value, string lineNo, string chkOperator, string mode)
        {
            var arrValue = Value.Split(',');
            for (int i = 0; i < arrValue.Length; i++)
            {
                if (Regex.IsMatch(arrValue[i].Replace(Global.Space, string.Empty), (@"[\<\>\]\[\""]")))
                {
                    ErrMsg.Add(new ErrorMsg(lineNo.ToString(), L("InvaliCharsMsg")));
                    return false;
                }
            }
            if (chkOperator.Contains(Global.LikeWithUpper))
            {
                for (int i = 0; i < arrValue.Length; i++)
                {
                    if (arrValue[i].IndexOf(Global.NewLineConstant) > -1)
                    {
                        ErrMsg.Add(new ErrorMsg(lineNo.ToString(), L("NewLineCharsMsg")));
                        return false;
                    }
                }
                if (Value.IndexOf(Global.CommaWithoutSpace) > -1)
                {
                    ErrMsg.Add(new ErrorMsg(lineNo.ToString(), L("CommaMsg")));
                    return false;
                }
                return true;
            }
            else if (chkOperator.Contains(Global.Between))
            {
                var parsedValuesArray = Value.Split('-');
                for (int i = 0; i < parsedValuesArray.Length;)
                {
                    if (parsedValuesArray[i].IndexOf(Global.NewLineConstant) > -1)
                    {
                        ErrMsg.Add(new ErrorMsg((lineNo).ToString(), L("NewLineCharsMsg")));
                        return false;
                    }
                    if (Value.Contains(",") && arrValue.Length == 2 && mode.ToString().ToUpper() == "G" && !Value.Contains("-"))
                        return true;
                    if (parsedValuesArray[i].IndexOf(Global.CommaWithoutSpace) > -1)
                    {
                        ErrMsg.Add(new ErrorMsg((lineNo).ToString(), L("InvalidBetweenMsg")));
                        return false;
                    }
                    if (parsedValuesArray != null)
                    {
                        if ((parsedValuesArray.Length == 2) && parsedValuesArray[0].Length > 0 && parsedValuesArray[1].Length > 0 && !isGroupByKeyCode1)
                            return true;
                        else
                        {
                            if (isGroupByKeyCode1)
                            {
                                if ((parsedValuesArray.Length == 2) && (parsedValuesArray[0]).Length > 0 && (parsedValuesArray[1]).Length > 0)
                                    return true;
                            }
                            ErrMsg.Add(new ErrorMsg((lineNo).ToString(), L("DashBetweenMsg")));
                            return false;
                        }
                    }
                    else
                    {
                        ErrMsg.Add(new ErrorMsg((lineNo).ToString(), L("DashBetweenMsg")));
                        return false;
                    }
                }

            }
            else if (chkOperator.Contains(Global.IN))
            {
                var parsedValuesArray = Value.Split('-');

                for (int i = 0; i < parsedValuesArray.Length; i++)
                {
                    if (parsedValuesArray[i].IndexOf(Global.NewLineConstant) > -1)
                    {
                        ErrMsg.Add(new ErrorMsg((lineNo).ToString(), L("NewLineCharsMsg")));
                        return false;
                    }
                    if (parsedValuesArray.Length == 2 && (parsedValuesArray[0]).Length > 0 && (parsedValuesArray[1]).Length > 0)
                    {
                        if (parsedValuesArray[i].IndexOf(",") == -1)
                        {
                            ErrMsg.Add(new ErrorMsg((lineNo).ToString(), L("InvalidINNotINMsg")));
                            return false;
                        }
                        else
                        {
                            var parsedValuesCommaArray = Value.Split(',');
                            if (parsedValuesCommaArray != null)
                            {
                                if (parsedValuesCommaArray[0].Length == 0 || parsedValuesCommaArray[1].Length == 0)
                                {
                                    ErrMsg.Add(new ErrorMsg((lineNo).ToString(), L("InvalidINNotINMsg")));
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
        private string ValidateZipRadius(string enteredValue)
        {
            var Valid = string.Empty;
            var value = enteredValue.Replace("\n", ",");
            if (value.EndsWith(","))
                value = value.Substring(0, value.Length - 1);
            var values = value.Split(',');
            var isGlobalRedious = false;
            foreach (string val in values)
            {
                if (val.StartsWith("radius") && values[0].StartsWith("radius"))
                {
                    isGlobalRedious = true;
                    var _val = val.Replace("radius", string.Empty).Replace(Global.EqualToOperator, string.Empty).Trim();
                    if (!Regex.IsMatch(_val, @"^\d{1,}(\.\d{1,2})?$"))
                        return L("ValidateZipRadiusDecimalMsg");
                }
                else if (!isGlobalRedious && !Regex.IsMatch(val, @"^[a-zA-Z0-9]{5,6}([\-][0-9]+(\.[0-9]{1,2})?)+(\s{0,}\,\s{0,}[a-zA-Z0-9]{5,6}([\-][0-9]+(\.[0-9]{1,2})?))*$"))
                    return L("ValidateZipRadiusPostalMsg");
                else if (isGlobalRedious && !(val.Length == 5 && Regex.IsMatch(val, @"^\d{1,}(\.\d{1,2})?$")))
                    return L("WrongZipRadiusMsg");
            }
            return Valid;
        }
        private bool ValidateSegmentSelectionValidations(DataTable dt)
        {
            var listOfSelection = new ListSegmentFieldDetails();
            var lineno = string.Empty;
            var description = string.Empty;
            try
            {
                for (int miCounter = 0; miCounter < dt.Rows.Count && ErrMsg.Count() < 5; miCounter++)
                {
                    lineno = !isDefaultFormat || isGroupByKeyCode1 ? string.Empty : miCounter.ToString();
                    listOfSelection.listFieldName = new List<string>();
                    listOfSelection.listValueOperator = new List<string>();
                    listOfSelection.listValues = new List<string>();

                    if (ErrMsg.Count >= 5)
                        return false;

                    if (dt.Columns.Contains("FIELD NAME"))
                        FillSegmentFieldDetails(Convert.ToString(dt.Rows[miCounter]["FIELD NAME"]), Convert.ToString(dt.Rows[miCounter]["VALUES"]), !dt.Columns.Contains("Value Operator") ? string.Empty : Convert.ToString(dt.Rows[miCounter]["VALUE OPERATOR"]), listOfSelection);
                    if (dt.Columns.Contains("FIELD NAME1"))
                    {
                        if ((string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["FIELD NAME1"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["VALUES1"]))) || (!String.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["FIELD NAME1"])) && string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["VALUES1"]))))
                            ErrMsg.Add(new ErrorMsg(lineno, L("FieldNamesValidations", 1)));
                        else
                            FillSegmentFieldDetails(Convert.ToString(dt.Rows[miCounter]["FIELD NAME1"]), Convert.ToString(dt.Rows[miCounter]["VALUES1"]), !dt.Columns.Contains("Value Operator1") ? string.Empty : Convert.ToString(dt.Rows[miCounter]["VALUE OPERATOR1"]), listOfSelection);
                    }
                    if (dt.Columns.Contains("FIELD NAME2"))
                    {
                        if ((string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["FIELD NAME2"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["VALUES2"]))) || (!String.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["FIELD NAME2"])) && string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["VALUES2"]))))
                            ErrMsg.Add(new ErrorMsg(lineno, L("FieldNamesValidations", 2)));
                        else
                            FillSegmentFieldDetails(Convert.ToString(dt.Rows[miCounter]["FIELD NAME2"]), Convert.ToString(dt.Rows[miCounter]["VALUES2"]), !dt.Columns.Contains("Value Operator2") ? string.Empty : Convert.ToString(dt.Rows[miCounter]["VALUE OPERATOR2"]), listOfSelection);
                    }
                    if (dt.Columns.Contains("FIELD NAME3"))
                    {
                        if ((string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["FIELD NAME3"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["VALUES3"]))) || (!String.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["FIELD NAME3"])) && string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["VALUES3"]))))
                            ErrMsg.Add(new ErrorMsg(lineno, L("FieldNamesValidations", 3)));
                        else
                            FillSegmentFieldDetails(Convert.ToString(dt.Rows[miCounter]["FIELD NAME3"]), Convert.ToString(dt.Rows[miCounter]["VALUES3"]), !dt.Columns.Contains("Value Operator3") ? string.Empty : Convert.ToString(dt.Rows[miCounter]["VALUE OPERATOR3"]), listOfSelection);
                    }
                    if (dt.Columns.Contains("FIELD NAME4"))
                    {
                        if ((string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["FIELD NAME4"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["VALUES4"]))) || (!String.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["FIELD NAME4"])) && string.IsNullOrEmpty(Convert.ToString(dt.Rows[miCounter]["VALUES4"]))))
                            ErrMsg.Add(new ErrorMsg(lineno, L("FieldNamesValidations", 4)));
                        else
                            FillSegmentFieldDetails(Convert.ToString(dt.Rows[miCounter]["FIELD NAME4"]), Convert.ToString(dt.Rows[miCounter]["VALUES4"]), !dt.Columns.Contains("Value Operator4") ? string.Empty : Convert.ToString(dt.Rows[miCounter]["VALUE OPERATOR4"]), listOfSelection);
                    }


                    if (ErrMsg.Count == 0)
                    {
                        for (int inner = 0; inner < listOfSelection.listFieldName.Count(); inner++)
                        {
                            selDetails = new SegmentSelectionDto();
                            var cfieldType = string.Empty;
                            selDetails.iGroupOrder = 1;
                            selDetails.cGrouping = "N";
                            selDetails.iGroupNumber = dt.Columns.Contains("Group Number") ? Convert.ToInt32(dt.Rows[miCounter]["GROUP NUMBER"]) : inner + 1;
                            if (dt.Columns.Contains("Field Name"))
                            {
                                var buildTableLayoutDetails = new SelectionDetails();
                                if (listOfSelection.listFieldName[inner].ToUpper() != string.Empty)
                                {
                                    selDetails.cQuestionFieldName = listOfSelection.listFieldName[inner];

                                    buildTableLayoutDetails = dt.Columns.Contains("Table Name") ? _customSegmentSelectionRepository.GetSegmentSelectionByOrderID(campaignId, selDetails.cQuestionFieldName, dt.Rows[miCounter]["Table Name"].ToString()).FirstOrDefault() : _customSegmentSelectionRepository.GetSegmentSelectionByOrderID(campaignId, selDetails.cQuestionFieldName, null).FirstOrDefault();
                                    if (buildTableLayoutDetails != null)
                                    {
                                        if (buildTableLayoutDetails.cValueMode == "N")
                                        {
                                            ErrMsg.Add(new ErrorMsg(lineno, L("ValueFMode", 3)));
                                            continue;
                                        }
                                        else
                                        {
                                            cfieldType = buildTableLayoutDetails.cFieldType;
                                            selDetails.cValueMode = buildTableLayoutDetails.cValueMode;
                                            selDetails.cTableName = buildTableLayoutDetails.cTableName;
                                        }
                                    }
                                    else
                                    {
                                        ErrMsg.Add(new ErrorMsg(lineno, L("BuildTableLayoutValidationsMsg", selDetails.cQuestionFieldName)));
                                        continue;
                                    }
                                }
                            }
                            if (dt.Columns.Contains("Value Operator"))
                            {
                                var values = listOfSelection.listValues[inner].ToUpper();
                                var _valOp = listOfSelection.listValueOperator[inner];
                                var fieldName = listOfSelection.listFieldName[inner].ToUpper();
                                ValidateValuesOperator(values, _valOp, fieldName.ToUpper(), lineno, cfieldType);
                            }
                            else
                                selDetails.cValueOperator = Global.IN;

                            if (dt.Columns.Contains("Values"))
                            {
                                var valMode = string.Empty;
                                var exValue = listOfSelection.listValues[inner].ToUpper();
                                if (!string.IsNullOrEmpty(selDetails.cValueMode))
                                    valMode = selDetails.cValueMode;
                                if (exValue != string.Empty)
                                {
                                    var strVal = exValue.Split('\n');
                                    var strValues = exValue.Split(',');
                                    var limit = Convert.ToInt32(_idmsConfigurationCache.GetConfigurationValue("BulkSegValueLimit", databaseId)?.cValue);
                                    if (limit > 0)
                                    {
                                        if (strVal.Length > limit || strValues.Length > limit)
                                            ErrMsg.Add(new ErrorMsg(lineno, L("ValidateLimitMsg", limit)));

                                    }
                                    else if (strVal.Length > 500 || strValues.Length > 500)
                                        ErrMsg.Add(new ErrorMsg(lineno, L("ValidateLimitMsg", 500)));

                                    else if (exValue.Contains('\n'))
                                    {
                                        var sb = new StringBuilder(exValue);
                                        sb.Replace('\n', ',');
                                        strValues = sb.ToString().Split(',');
                                    }
                                    else
                                        strValues = exValue.Split(',');

                                    if (listOfSelection.listFieldName[inner].ToUpper() == "ZIPRADIUS")
                                    {
                                        var validateMsg = ValidateZipRadius(exValue.ToLower().Trim());

                                        if (!string.IsNullOrEmpty(validateMsg))
                                        {
                                            ErrMsg.Add(new ErrorMsg(lineno, validateMsg));
                                            continue;
                                        }
                                        else
                                        {
                                            selDetails.cQuestionFieldName = "ZIP";
                                            selDetails.cQuestionDescription = "Zip Radius";
                                            selDetails.cValues = exValue.Contains('\n') ? exValue.Replace('\n', ',') : exValue;
                                        }
                                    }
                                    else if (listOfSelection.listFieldName[inner].ToUpper() == "GEORADIUS")
                                    {
                                        if (ValidateGeoRadius(exValue, miCounter.ToString()) && !(exValue.Trim().IndexOf('|') > 0))
                                        {
                                            bool result;
                                            var latlongValue = string.Empty;
                                            var zipradius = string.Empty;
                                            var addressDetails = new AddressInputDto
                                            {
                                                DatabaseId = databaseId,
                                                MainTableName = selDetails.cTableName,
                                                AddressFilter = exValue.Trim()
                                            };
                                            result = ValidateGeoRadiusByMatchLevel(addressDetails, miCounter.ToString(), out zipradius, out description, out latlongValue);

                                            if (result && !string.IsNullOrEmpty(description))
                                            {
                                                if (!string.IsNullOrEmpty(zipradius))
                                                {
                                                    selDetails.cValues = zipradius;
                                                    selDetails.cQuestionDescription = "Zip Radius";
                                                    selDetails.cQuestionFieldName = "ZIP";
                                                    selDetails.cValueMode = "T";
                                                }
                                                else
                                                    selDetails.cValues = latlongValue;
                                                selDetails.cDescriptions = description;
                                            }
                                        }
                                        else
                                            selDetails.cValues = exValue;
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(selDetails.cValueOperator))
                                        {
                                            if (ValidateText(listOfSelection.listValues[inner].ToUpper(), lineno, (selDetails.cValueOperator).ToUpper(), valMode))
                                                selDetails.cValues = listOfSelection.listValues[inner].ToUpper();
                                        }
                                    }
                                }
                            }

                            selDetails.cJoinOperator = Global.AndOperator.ToUpper();
                            selDetails = CommonHelpers.ConvertNullStringToEmptyAndTrim(selDetails);
                            lstSegDetails.Add(selDetails);
                        }
                    }
                }
                //if (ErrMsg.Count() > 5)
                //    return false;
                return ErrMsg.Count() > 0 ? false : true;
            }
            catch (Exception ex)
            {
                ErrMsg.Add(new ErrorMsg(lineno, ex.Message));
                return false;
            }

        }
        private bool ValidateGeoRadius(string exValue, string lineNo)
        {
            var checkForRadius = true;
            var re = new Regex(@"^?\d+\.\d+\|-?\d+\.\d+\:\d+(\.\d{1,2})?$");
            var iBlanks = 0;
            var isFirst = true;
            if (exValue.Split(':').Length > 2)
            {
                ErrMsg.Add(new ErrorMsg(lineNo.ToString(), L("GeoMultiple")));
                return false;
            }
            if (exValue.Trim() == string.Empty)
                iBlanks++;
            if (isFirst)
            {
                isFirst = false;
                checkForRadius = exValue.IndexOf('|') > 0;
                if (!checkForRadius)
                    re = new Regex(@"(\:+[0-9]*(\.[0-9]{1,2})?)$");
            }
            if (checkForRadius)
            {
                if (!re.IsMatch(exValue) || exValue.IndexOf('|') < 0)
                {
                    ErrMsg.Add(new ErrorMsg(lineNo.ToString(), L("GeoFormatError")));
                    return false;
                }
            }
            else
            {
                if (!re.IsMatch(exValue) || exValue.IndexOf('|') >= 0)
                {
                    ErrMsg.Add(new ErrorMsg((lineNo).ToString(), L("RadiusErrorMsg")));
                    return false;
                }
            }
            return true;
        }
        private void ValidateValuesOperator(string values, string valueOpertor, string fieldName, string miCounter, string cFieldType)
        {
            string[] enteredVal = null;
            var count = 0;
            var _valOp = Convert.ToString(valueOpertor).ToUpper();

            if (string.IsNullOrEmpty(_valOp))
            {
                _valOp = Global.IN;
                selDetails.cValueOperator = _valOp;
            }
            else
            {
                if (fieldName == "ZIPRADIUS" || fieldName == "GEORADIUS" || cFieldType == "M")
                {
                    if (!IsValidOperator(InvalidOperatorsList, _valOp))
                        ErrMsg.Add(new ErrorMsg(miCounter, L("InvalidOperatorUsedForFields", fieldName)));
                    else
                        selDetails.cValueOperator = _valOp;
                }
                else if (selDetails.cValueMode == "G")
                {
                    if (values.Contains("/n"))
                    {
                        enteredVal = values.Split('\n');
                        count = enteredVal.Length;
                    }
                    else if (values.Contains(","))
                    {
                        enteredVal = values.Split(',');
                        count = enteredVal.Length;
                    }
                    else if (values.Contains("-"))
                    {
                        enteredVal = values.Split('-');
                        count = enteredVal.Length;
                    }
                    var InvalidOperators = new string[] { Global.LikeWithUpper, Global.NotLike };
                    if ((_valOp.IndexOf(Global.GreaterThanOperator) > -1 || _valOp.IndexOf(Global.LessThanOperator) > -1 || _valOp.Contains(Global.Greater) || _valOp.Contains(Global.GreaterEqual) || _valOp.Contains(Global.Less) || _valOp.Contains(Global.LessEqual)) && count > 1)
                        ErrMsg.Add(new ErrorMsg(miCounter, L("SelectOnlyOneValueMsg")));
                    else if (_valOp.Contains(Global.Between) && count != 2)
                        ErrMsg.Add(new ErrorMsg(miCounter, L("BweteenOperatorMsg")));

                    else if (cFieldType == "M")
                    {
                        if (_valOp.IndexOf(Global.GreaterThanOperator) > -1 || _valOp.IndexOf(Global.LessThanOperator) > -1 || _valOp.Contains(Global.Greater) || _valOp.Contains(Global.GreaterEqual) || _valOp.Contains(Global.Less) || _valOp.Contains(Global.LessEqual))
                            ErrMsg.Add(new ErrorMsg(miCounter, L("InvalidOperator")));
                    }
                    else if (!IsValidOperator(InvalidOperators, _valOp))
                        ErrMsg.Add(new ErrorMsg(miCounter, L("InvalidOperatorUsedForFields", fieldName)));
                    else
                        selDetails.cValueOperator = _valOp;
                }
                else if (selDetails.cValueMode == "T")
                {
                    if (!IsValidOperator(new string[] { }, _valOp))
                        ErrMsg.Add(new ErrorMsg(miCounter, L("InvalidOperator")));

                    else
                    {
                        if (_valOp == Global.LikeWithUpper || _valOp == Global.NotLike || _valOp == Global.Between || _valOp == Global.NotBetween || _valOp == Global.IN ||
                            _valOp == Global.NotIN ||
                            _valOp == Global.Greater ||
                            _valOp == Global.GreaterEqual ||
                            _valOp == Global.Less ||
                            _valOp == Global.LessEqual ||
                            _valOp == Global.GreaterThanOperator ||
                            _valOp == Global.GreaterThanEqualToOperator ||
                            _valOp == Global.LessThanOperator ||
                            _valOp == Global.LessThanEqualToOperator)
                        {
                            if (_valOp == Global.Greater)
                                selDetails.cValueOperator = Global.GreaterThanOperator;
                            else if (_valOp == Global.GreaterEqual)
                                selDetails.cValueOperator = Global.GreaterThanEqualToOperator;
                            else if (_valOp == Global.Less)
                                selDetails.cValueOperator = Global.LessThanOperator;
                            else if (_valOp == Global.LessEqual)
                                selDetails.cValueOperator = Global.LessThanEqualToOperator;
                            else
                                selDetails.cValueOperator = _valOp;
                        }
                        else if (cFieldType == "M")
                        {
                            if (_valOp.IndexOf(Global.GreaterThanOperator) > -1 || _valOp.IndexOf(Global.Less) > -1 || _valOp.Contains(Global.Greater) || _valOp.Contains(Global.GreaterEqual) || _valOp.Contains(Global.Less) || _valOp.Contains(Global.LessEqual))
                                ErrMsg.Add(new ErrorMsg(miCounter, L("InvalidOperator")));
                        }
                        else
                            ErrMsg.Add(new ErrorMsg(miCounter, L("InvalidOperator")));

                    }
                }
                else
                    ErrMsg.Add(new ErrorMsg(miCounter, L("InvalidOperator")));

            }
        }
        #endregion

        #region SaveSegemnt
        private async Task<bool> SaveSegmentRules(DataTable dt)
        {
            try
            {
                var listOfSegments = new List<Segment>();                
                if (dt.Rows.Count > 0)
                {
                    if(ErrMsg.Count<=0)
                    {                   
                    
                        for (int miCounter = 0; miCounter < dt.Rows.Count; miCounter++)
                        {
                            var segment = new Segment();
                            string _titleSupp, _FixedTitle;
                            _titleSupp = _FixedTitle = string.Empty;

                            segment.OrderId = campaignId != 0 ? campaignId : 0;
                            segment.iDedupeOrderSpecified = dt.Columns.Contains("Segment No") && !string.IsNullOrEmpty((dt.Rows[miCounter]["SEGMENT NO"]).ToString()) ? Convert.ToInt32(dt.Rows[miCounter]["SEGMENT NO"]) : 0;
                            segment.cDescription = dt.Columns.Contains("description") ? (dt.Rows[miCounter]["DESCRIPTION"]).ToString() : string.Empty;
                            segment.iRequiredQty = dt.Columns.Contains("Required Quantity") && !string.IsNullOrEmpty(dt.Rows[miCounter]["REQUIRED QUANTITY"].ToString().Trim()) ? Convert.ToInt32(dt.Rows[miCounter]["REQUIRED QUANTITY"]) : 0;
                            segment.cKeyCode1 = dt.Columns.Contains("Keycode1") ? (dt.Rows[miCounter]["KEYCODE1"]).ToString() : string.Empty;
                            segment.cKeyCode2 = dt.Columns.Contains("Keycode2") ? (dt.Rows[miCounter]["KEYCODE2"]).ToString() : string.Empty;
                            segment.iDoubleMultiBuyerCount = dt.Columns.Contains("Double Multibuyer") && !string.IsNullOrEmpty(dt.Rows[miCounter]["DOUBLE MULTIBUYER"].ToString().Trim()) ? Convert.ToInt32(dt.Rows[miCounter]["DOUBLE MULTIBUYER"]) : 0;
                            segment.cMaxPerGroup = dt.Columns.Contains("MaxPer Group") ? (dt.Rows[miCounter]["MAXPER GROUP"]).ToString() : "00";
                            segment.iUseAutosuppress = dt.Columns.Contains("Auto Suppress") ? Convert.ToBoolean(dt.Rows[miCounter]["AUTO SUPPRESS"]) : true;
                            segment.iIsRandomRadiusNth = dt.Columns.Contains("Random Radius Nth") ? Convert.ToBoolean(dt.Rows[miCounter]["RANDOM RADIUS NTH"]) : false;
                            segment.iGroup = dt.Columns.Contains("Net Group") ? Convert.ToInt32(dt.Rows[miCounter]["NET GROUP"]) : 1;
                            segment.iOutputQty = -1;
                            segment.iAvailableQty = 0;

                            segment.cNthPriorityField = string.Empty;
                            segment.cNthPriorityFieldOrder = string.Empty;

                            if (dt.Columns.Contains("Title Suppression"))
                                _titleSupp = (dt.Rows[miCounter]["TITLE SUPPRESSION"]).ToString().ToUpper();
                            if (dt.Columns.Contains("Fixed Title"))
                                _FixedTitle = (dt.Rows[miCounter]["FIXED TITLE"]).ToString().ToUpper();

                            if (string.IsNullOrEmpty(_titleSupp))
                            {
                                segment.cTitleSuppression = "B";
                                segment.cFixedTitle = string.Empty;
                            }
                            else
                            {
                                switch (Convert.ToString(_titleSupp).ToUpper())
                                {
                                    case "N":
                                    case "F":
                                    case "O":
                                        {
                                            segment.cTitleSuppression = _titleSupp;
                                            segment.cFixedTitle = _FixedTitle;
                                            break;
                                        }
                                    case "B":
                                    case "T":
                                        {
                                            segment.cTitleSuppression = _titleSupp.Trim().ToUpper();
                                            segment.cFixedTitle = string.Empty;
                                            break;
                                        }

                                    default:
                                        throw new Exception(L("DefaultCaseError"));
                                }
                            }
                            segment.cCreatedBy = _mySession.IDMSUserName;
                            segment.dCreatedDate = DateTime.Now;
                            segment = CommonHelpers.ConvertNullStringToEmptyAndTrim(segment);
                            listOfSegments.Add(_segmentRepository.Insert(segment));    
                        }
                    }
                    CurrentUnitOfWork.SaveChanges();                    
                    listOfSegments = listOfSegments.OrderBy(x => x.iDedupeOrderSpecified).ToList();                    
                    foreach (var item in listOfSegments)
                    {
                        segList.Add(item.Id);
                    }                    
                    if (segList != null && ErrMsg.Count == 0)
                        await SaveSegmentSelection();
                }
                return ErrMsg.Count() > 0 ? false : true;

            }
            catch (Exception ex)
            {
                ErrMsg.Add(new ErrorMsg(string.Empty, L("ErrorSegmentSelection", ex)));
                return false;
            }
        }
        #endregion

        #region SaveSegmentSelection
        private async Task SaveSegmentSelection()
        {
            
            if (ErrMsg.Count <= 0 && lstSegDetails.Count > 0)
            {
                int i;
                var k = 0;
                for (i = 0; i < lstSegDetails.Count(); i++)
                {
                    if (lstSegDetails[i].iGroupNumber == 1)
                    {
                        lstSegDetails[i].SegmentId = segList[k];
                        k++;
                    }
                    else if (lstSegDetails[i].iGroupNumber > 1)
                        lstSegDetails[i].SegmentId = lstSegDetails[i - 1].SegmentId;
                }
                
                var savedSelectionData = new SegmentSelectionSaveDto
                {
                    DatabaseId = databaseId,
                    selections = lstSegDetails,
                    campaignId = campaignId
                };
               
                await CreateSegmentSelectionDetails(savedSelectionData);
                

            }
        }
        #endregion

        #region DownLoadTemplate
        public FileDto DownloadTemplate(bool isDefaultFormat, int databaseId)
        {
            const string RadioTvStationFileName = "BulkSegmentUploadTemplateRadioStation.xlsx";
            const string BulkStationFileName = "BulkSegmentUploadTemplate.xlsx";
            try
            {
                var xlsxFileName = isDefaultFormat ? RadioTvStationFileName : BulkStationFileName;
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseId);
                var fileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet;
                if (awsFlag)
                {
                    var fileDownloadPath = _idmsConfigurationCache.GetConfigurationValue("TemplatePathAWS", databaseId).cValue;
                    return new FileDto($"{fileDownloadPath}/{xlsxFileName}", fileType, xlsxFileName, isAWS: awsFlag);
                }
                else
                {
                    var templatePath = _idmsConfigurationCache.GetConfigurationValue("TemplatePath", 0).cValue ?? string.Empty;
                    var _filePath = string.Format("{0}\\{1}", templatePath, xlsxFileName);
                    return new FileDto(_filePath, fileType, xlsxFileName);
                }                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}
