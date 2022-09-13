using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SecurityGroups.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Abp.UI;
using System.Collections.Generic;
using System.Data.SqlClient;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.GroupBrokers.Dtos;
using System.Linq;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Common.Exporting;
using Syncfusion.XlsIO;
using System.IO;
using Abp.AspNetZeroCore.Net;

namespace Infogroup.IDMS.SecurityGroups
{
    [AbpAuthorize(AppPermissions.Pages_SecurityGroups)]
    public class SecurityGroupsAppService : IDMSAppServiceBase, ISecurityGroupsAppService
    {
        private readonly IRepository<Database, int> _databaseRepository;
        private readonly IRepository<SecurityGroup> _securityGroupRepository;
        private readonly ISecurityGroupRepository _customSecurityGroupRepository;
        private readonly AppSession _mySession;

        public SecurityGroupsAppService(
            IRepository<Database, int> databaseRepository,
            IRepository<SecurityGroup> securityGroupRepository,
            ISecurityGroupRepository customSecurityGroupRepository,
            AppSession mySession
            )
		{
            _databaseRepository = databaseRepository;
            _securityGroupRepository = securityGroupRepository;
            _customSecurityGroupRepository = customSecurityGroupRepository;
            _mySession = mySession;
		}

        #region Fetch Security Groups
        public PagedResultDto<SecurityGroupDto> GetAllSecurityGroups(GetAllSecurityGroupsInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var query = GetAllSecurityGroupsQuery(input);
                var result = _customSecurityGroupRepository.GetAllSecurityGroupsList(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task<CreateOrEditSecurityGroupDto> GetSecurityGroupForEdit(EntityDto input)
        {
            try
            {
                var securityGroup = await _securityGroupRepository.FirstOrDefaultAsync(input.Id);
                var editsecurityGroupData = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditSecurityGroupDto>(securityGroup));
                return editsecurityGroupData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Save
        [AbpAuthorize(AppPermissions.Pages_SecurityGroups_Create, AppPermissions.Pages_SecurityGroups_Edit)]
        public async Task CreateOrEdit(CreateOrEditSecurityGroupDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                GroupNameValidation(input);
                if (input.Id == 0)
                {
                    input.cStatus = "A";
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var securityGroup = ObjectMapper.Map<SecurityGroup>(input);
                    await _securityGroupRepository.InsertAsync(securityGroup);
                }
                else
                {
                    var updateSecurityGroup = _securityGroupRepository.Get(input.Id);
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateSecurityGroup);
                    CurrentUnitOfWork.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public void GroupNameValidation(CreateOrEditSecurityGroupDto input)
        {
            try
            {
                if(input.Id == 0)
                {
                    var isExistingGroupName = _customSecurityGroupRepository.GetAll().Any(p => p.cGroupName == input.cGroupName && p.DatabaseID == input.DatabaseId);
                    if (isExistingGroupName) throw new UserFriendlyException(L("GroupNameValidation"));
                }
                else
                {
                    var isExistingGroupName = _customSecurityGroupRepository.GetAll().Any(p => p.cGroupName == input.cGroupName && p.DatabaseID == input.DatabaseId && p.Id != input.Id);
                    if (isExistingGroupName) throw new UserFriendlyException(L("GroupNameValidation"));
                }
                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Export
        [AbpAuthorize(AppPermissions.Pages_SecurityGroups_Print)]
        public FileDto ExportToExcel(GetAllSecurityGroupsInput input)
        {
            try
            {
                //Create XLSX file throgh syncFusion
                var parentList = GetAllSecurityGroups(input);
                var excelData = parentList.Items.Select(group =>
                {
                    var groupExcelDto = ObjectMapper.Map<ExcelExportGroupsDto>(group);
                    groupExcelDto.GroupBrokerList = _customSecurityGroupRepository.GetAllGroupBroker(group.Id);
                    groupExcelDto.GroupUsers = _customSecurityGroupRepository.GetAllGroupUsers(group.Id);
                    return groupExcelDto;
                }).ToList();
                var databaseName = _databaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseName")}:{databaseName}";
                var fileName = $"{L("SecurityGroupFileName")}";

                return AllGroupExportToFile(excelData, databaseName, fileName);                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private FileDto AllGroupExportToFile(List<ExcelExportGroupsDto> excelDetails, string databaseName, string fileName)
        {
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    var application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Excel2016;

                    //Create a workbook
                    var workbook = application.Workbooks.Create(1);
                    var worksheet = workbook.Worksheets[0];

                    //change sheet name
                    workbook.Worksheets[0].Name = L("SecurityGroupSheetName");

                    //Disable gridlines in the worksheet
                    worksheet.IsGridLinesVisible = false;

                    //Enter values to the cells from A1
                    worksheet.Range["A1"].Text = databaseName;

                    //Header value
                    worksheet.Range["A2"].Text = L("ID");
                    worksheet.Range["B2"].Text = L("cGroupName");
                    worksheet.Range["C2"].Text = L("isActive");

                    //Loop for display data
                    var rowIndex = 3;
                    foreach (var item in excelDetails)
                    {
                        worksheet.Range[$@"A{rowIndex}"].Text = item.Id.ToString();
                        worksheet.Range[$@"B{rowIndex}"].Text = item.cGroupName.Trim();
                        worksheet.Range[$@"C{rowIndex}"].Text = Convert.ToBoolean(item.iIsActive).ToString().ToUpper();
                        worksheet.Range[$@"D{rowIndex}"].Text = L("BrokerSheetCell");
                        worksheet.Range[$@"D{rowIndex}"].CellStyle.Font.Underline = ExcelUnderline.Single;
                        if (item.GroupBrokerList.Count != 0)
                        {
                            foreach (var broker in item.GroupBrokerList)
                            {
                                rowIndex++;
                                worksheet.Range[$@"D{rowIndex}"].Text = broker;
                            }
                        }
                        else
                        {
                            rowIndex++;
                        }
                        rowIndex++;
                        worksheet.Range[$@"D{rowIndex}"].Text = L("Users");
                        worksheet.Range[$@"D{rowIndex}"].CellStyle.Font.Underline = ExcelUnderline.Single;
                        if (item.GroupUsers.Count != 0)
                        {
                            foreach (var user in item.GroupUsers)
                            {
                                rowIndex++;
                                worksheet.Range[$@"D{rowIndex}"].Text = user;
                            }
                        }
                        else
                        {
                            rowIndex++;
                        }
                        rowIndex++;
                        worksheet.Range[$@"A{rowIndex}:E{rowIndex}"].Merge();
                        rowIndex++;
                    }

                    //Alignment
                    worksheet.Range[$@"A2:A{rowIndex}"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range[$@"C2:C{rowIndex}"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.UsedRange[2, 1, 2, 5].CellStyle.ColorIndex = ExcelKnownColors.Yellow;
                    worksheet.SetColumnWidth(1, 22.00);
                    worksheet.SetColumnWidth(2, 64.14);
                    worksheet.SetColumnWidth(3, 15.57);
                    worksheet.SetColumnWidth(4, 72.43);

                    using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        workbook.SaveAs(fileStream);
                        workbook.Close();
                    }

                    var fileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet;
                    return new FileDto(fileName, fileType, true);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region Count Users
        public PagedResultDto<UserCountDto> GetAllUserCount(GetAllUserCountsInputDto input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var query = GetAllUserCountQuery(input);
                var result = _customSecurityGroupRepository.GetAllUserCountList(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Security Group Business
        private static Tuple<string, string, List<SqlParameter>> GetAllSecurityGroupsQuery(GetAllSecurityGroupsInput filters)
        {

            string[] filtersarray = null;
            var isOrderId = Validation.ValidationHelper.IsNumeric(filters.Filter);
            if (!string.IsNullOrEmpty(filters.Filter))
            {
                filtersarray = filters.Filter.Split(',');
            }

            var GroupNameAndDescriptionFilter = $@"AND (G.cGroupName LIKE @FilterText OR G.cGroupDescription LIKE @FilterText)";

            var query = new Common.QueryBuilder();
            query.AddSelect($"G.ID, G.DatabaseID, D.cDatabaseName, G.cGroupName, ISNULL(G.iIsActive,0) iIsActive, (SELECT COUNT(*) FROM tblUserGroup INNER JOIN tblUser ON tblUserGroup.UserID = tblUser.ID AND tblUser.iIsActive = 1 WHERE tblUserGroup.GroupID = G.ID) AS USERCOUNT ,G.cGroupDescription, G.cCreatedBy, G.dCreatedDate, G.cModifiedBy, G.dModifiedDate");
            query.AddFrom("tblGroup", "G");
            query.AddJoin("tblDatabase", "D", "DatabaseID", "G", "INNER JOIN", "ID");
            query.AddWhere("", "G.cStatus", "EQUALTO", "A");
            query.AddWhere("AND", "G.DatabaseID", "EQUALTO", filters.SelectedDatabase.ToString());

            if (isOrderId)
                query.AddWhere("AND", "G.ID", "IN", filtersarray);
            else
            {
                query.AddWhereString(GroupNameAndDescriptionFilter);
            }
            query.AddSort(filters.Sorting ?? "cGroupName ASC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }

        private static Tuple<string, string, List<SqlParameter>> GetAllUserCountQuery(GetAllUserCountsInputDto filters)
        {

            string[] filtersarray = null;
            if (!string.IsNullOrEmpty(filters.Filter))
            {
                filtersarray = filters.Filter.Split(',');
            }

            var FirstNameLastNameEmailFilter = $@"AND (U.cFirstName LIKE @FilterText OR U.cLastName LIKE @FilterText OR U.cEmail LIKE @FilterText)";

            var query = new Common.QueryBuilder();
            query.AddSelect($"U.cFirstName, U.cLastName, U.cEmail");
            query.AddFrom("tblUser", "U");
            query.AddJoin("tblUserGroup", "UG", "ID", "U", "INNER JOIN", "UserID");
            query.AddWhere("", "u.iIsActive", "EQUALTO", "1");
            query.AddWhere("AND", "ug.GroupID", "EQUALTO", filters.GroupID.ToString());
            
            query.AddWhereString(FirstNameLastNameEmailFilter);
            
            query.AddSort(filters.Sorting ?? "u.cLastName ASC");
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        #endregion
    }
}