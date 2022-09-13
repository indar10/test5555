using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.OrderExportParts.Dtos;
using Infogroup.IDMS.SelectionFieldCountReports.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.ShippedReports.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Campaigns
{
    public interface ICampaignRepository : IRepository<Campaign, int>
    {
        Task<PagedResultDto<GetCampaignsListForView>> GetAllCampaignsList(string input1, string input2, List<SqlParameter> sqlParameters, GetCampaignListFilters filters,string userName);
        Task<GetCampaignsListForView> getOfferMailerBuild(string Query, List<SqlParameter> sqlParameters);
        List<int> GetDatabaseIdByUserID(int userID);
        int GetDivisionIDFromOrderID(string Query, List<SqlParameter> sqlParameters);
        void CopyCampaign(CampaignDto copyCampaignParams);
        void CopyDivCampaign(CampaignDto copyCampaignDivParams);
        List<GetCampaignsOutputDto> GetAllOutputLayouts(int campaignId);
        List<GetCampaignsOutputDto> GetCampaignShipToValues(string Query, List<SqlParameter> sqlParameters);
        List<DropdownOutputDto> GetCampaignSortValue(int campaignId);
        GetCampaignsOutputDto GetFtpDetailsByCompanyIdAndDivisionrId(string Query, List<SqlParameter> sqlParameters);
        List<LayoutTemplateDto> GetOutputLayoutTemplate(int buildId, int campaignId);
        void CopyOrderExportLayout(int iOrderID, int iExportLayoutID, string sInitiatedBy);
        List<GetCampaignMultidimensionalReportForViewDto> GetMultiColumnFields(string iOrderID, string skippedFields);
        List<GetExportLayoutSelectedFieldsDto> GetExportLayoutSelectedFields(int buildId, int campaignId);
        Task<List<CampaignQueueDto>> GetAllCampaignQueue(int userId,string userName);
        List<int> GetExternalBuildTableIDByOrderID(int orderId);
        List<GetCampaignMultidimensionalReportForViewDto> GetExternalDatabaseFields();
        List<CampaignExportPartDto> GetExportPartsSelection(int campaignId, int part);
        bool CheckIfOutputFileExists(int campaignId);
        List<DropdownOutputDto> GetSalesRepDropdownValues(int userId = 0);
        int GetOESSStatusByCampaignID(int campaignId);
        PagedResultDto<GetShippedReportView> GetAllShippedReportsList(Tuple<string, string, List<SqlParameter>> query);
        BuildDetails GetBuildDetails(int campaignId);
        Task<GetCampaignsListForView> GetCampaignById(Tuple<string, List<SqlParameter>> query, string loginUsername);
        Task<DropdownOutputDto> GetCampaignStatusById(Tuple<string, List<SqlParameter>> query);
        PagedResultDto<GetSelectionFieldCountReportView> GetAllSelectionFieldCountReportsList(Tuple<string, string, List<SqlParameter>> query);        
        PagedResultDto<GetOrderDetailsView> GetOrderDetailsList(Tuple<string, string, List<SqlParameter>> query);
        List<GetCampaignsListForView> GetTopNCampaigns(string cDescription,string mailer,int numberOfCopies,string userName,int userID,string DatabaseID);
        Task<PagedResultDto<GetCampaignsListForView>> GetAllFastCountCampaignsList(string input1, string input2, List<SqlParameter> sqlParameters,string username);
    }
}
