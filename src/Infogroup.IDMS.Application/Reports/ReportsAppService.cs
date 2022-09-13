using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Infogroup.IDMS.Configuration;
using Microsoft.AspNetCore.Hosting;
using Infogroup.IDMS.Reports.Dtos;
using Infogroup.IDMS.UserReports;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using Abp.UI;
using Abp.Authorization;
using Infogroup.IDMS.Authorization;
using System.Data;
using Infogroup.IDMS.CampaignXTabReports;
using System.IO;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Common.Exporting;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Infogroup.IDMS.Reports
{
    [AbpAuthorize(AppPermissions.Pages_Dashboard, AppPermissions.Pages_FastCount_Create)]
    public class ReportsAppService : IDMSAppServiceBase, IReportsAppService
    {
        private readonly IRepository<UserReport> _userReportRepository;

        private readonly ICampaignXTabReportsRepository _customCampaignXtabReportsRepository;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly AppSession _mySession;
        private readonly ICommonExcelExporter _commonExcelExporter;

        // PBI Properties
        private string Username => _appConfiguration["Report:Username"];
        private string Password => _appConfiguration["Report:Password"];
        private string AuthorityUrl => _appConfiguration["Report:AuthorityUrl"];
        private string ResourceUrl => _appConfiguration["Report:ResourceUrl"];
        private string ApplicationId => _appConfiguration["Report:ApplicationId"];
        private string WorkspaceId => _appConfiguration["Report:WorkspaceId"];
        private string AuthEndpoint => _appConfiguration["Report:AuthEndpoint"];

        public ReportsAppService(IHostingEnvironment env,
            IRepository<Report> reportRepository,
            IRepository<UserReport> userReportRepository,
            ICommonExcelExporter commonExcelExporter,
            ICampaignXTabReportsRepository customCampaignXtabReportsRepository,
            AppSession mySession
            )
        {
            _appConfiguration = env.GetAppConfiguration();
            _userReportRepository = userReportRepository;
            _customCampaignXtabReportsRepository = customCampaignXtabReportsRepository;
            _commonExcelExporter = commonExcelExporter;
            _mySession = mySession;
        }
        private async Task<string> GetAzureADTokenAsync()
        {
            var token = string.Empty;
            var uri = $"{AuthEndpoint}/api/AuthenticationAPI?AuthorityUrl={AuthorityUrl}&ApplicationId={ApplicationId}&ResourceUrl={ResourceUrl}&Username={Username}&Password={Password}";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(uri);
                var json = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<string>(json);
                return token;
            }
        }

        public async Task<GetReportForViewDto> GetReports()
        {
            try
            {
                var result = new GetReportForViewDto
                {
                    Reports = (from userReport in _userReportRepository.GetAll()
                               where userReport.UserID == _mySession.IDMSUserId
                               orderby userReport.ReportFk.cReportName
                               select ObjectMapper.Map<ReportDto>(userReport.ReportFk)).ToList()
                };
                if (result.Reports.Count == 0) throw new UserFriendlyException(L("NoReportError"));
                result.ReportOptions = result.Reports
                    .Select(report => new DropdownOutputDto { Value = report.Id, Label = report.cReportName })
                    .ToList();
                result.SelectedReport = Convert.ToInt32(result.ReportOptions.First().Value);
                result.AccessToken = await GetAzureADTokenAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #region GetReportForFastCount
        public FileDto GenerateExcelReport(Dictionary<string, Object> excelData)
        {
            try
            {
                DataSet excelDataSet = new DataSet();
                foreach(var item in excelData)
                {
                    DataTable dt = new DataTable(item.Key);
                    if (item.Value.GetType() == typeof(JArray))
                    {
                        var jArray = item.Value as JArray;
                        var firstElem = jArray.Children<JObject>().First();
                        foreach (JProperty prop in firstElem.Properties())
                        {
                            dt.Columns.Add(prop.Name);
                        }

                        DataRow dr = null;
                        foreach (var jObject in jArray.Children<JObject>())
                        {
                            dr = dt.NewRow();
                            foreach (JProperty prop in jObject.Properties())
                            {
                                dr[prop.Name] = jObject[prop.Name];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    excelDataSet.Tables.Add(dt);
                }
                return _commonExcelExporter.FastCountReportAllExportToFile(excelDataSet);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region GetInstantBreakDown
        public async Task<DataSet> GetInstantBreakDown(string reportType, int segmentId, int campaignId)
        {
            try
            {
                DataTable dataTable = new DataTable();
                var reportCountResult = string.Empty;
                var Data = _customCampaignXtabReportsRepository.GetAllCampaignXtabReportIds(campaignId);
                DataSet ExcelDataSet = new DataSet();
                var endpointaddress = _appConfiguration["Services:Uri"];
                var service = new IDMSCommonService.IDMSIQServiceClient(endpointaddress);
                foreach (var reportID in Data)
                {
                    var getReportResponse = await service.GetReport(reportType, segmentId, Convert.ToInt32(reportID));
                    if (getReportResponse != null && getReportResponse.GetReportResult != null && getReportResponse.GetReportResult.Report != null && getReportResponse.GetReportResult.Report.Any1 != null)
                    {
                        string xmlContent = Convert.ToString(getReportResponse.GetReportResult.Report.Any1.FirstNode);
                        DataSet ds = new DataSet();
                        StringReader stringReader = new StringReader(xmlContent);
                        ds.ReadXml(stringReader);
                        dataTable = ds.Tables[0].Copy();
                        ExcelDataSet.Tables.Add(dataTable);
                        stringReader.Dispose();
                    }
                }
                return ExcelDataSet;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        public  List<ReportDto> GetAllForReportsDropdown()
        {
            var report = (from userReport in _userReportRepository.GetAll()
                          select
                          ObjectMapper.Map<ReportDto>(userReport.ReportFk)).ToList();
            var result = report.GroupBy(item => item.Id)
                        .Select(rep => rep.First())
                        .ToList();

            return result;
        }

        public async Task CreateOrEditUserReport(UserReport input)
        {

            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                if (input.Id == null)
                {
                    var idmsConfiguration = ObjectMapper.Map<UserReport>(input);
                    await _userReportRepository.InsertAsync(idmsConfiguration);
                }
                else
                {
                    var updateConfig = _userReportRepository.Get(input.Id);
                    ObjectMapper.Map(input, updateConfig);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}