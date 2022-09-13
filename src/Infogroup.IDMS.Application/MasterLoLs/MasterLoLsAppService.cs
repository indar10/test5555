using Infogroup.IDMS.Databases;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.MasterLoLs.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using System.Data.SqlClient;
using Infogroup.IDMS.Contacts;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.ListCASContacts;
using Infogroup.IDMS.ListCASContacts.Dtos;
using Infogroup.IDMS.ListMailers;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.ListMailerRequesteds;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Common.Exporting;
using Infogroup.IDMS.Brokers;
using Infogroup.IDMS.BuildLoLs;
using System.Text;
using System.Diagnostics;

namespace Infogroup.IDMS.MasterLoLs
{
    //[AbpAuthorize(AppPermissions.Pages_MasterLoLs)]
    [AbpAuthorize]
    public class MasterLoLsAppService : IDMSAppServiceBase, IMasterLoLsAppService
    {
        private readonly IMasterLolListofListsRepository _customMasterLoLRepository;
        private readonly IRepository<Database, int> _lookup_databaseRepository;
        private readonly IRepository<BuildLol, int> _buildLolRepository;
        private readonly AppSession _mySession;
        private readonly IRepository<ListCASContact, int> _listCasContactRepository;
        private readonly IRepository<ListMailer, int> _listMailerRepository;
        private readonly IRepository<ListMailerRequested, int> _listReqMailerRepository;
        private readonly IShortSearch _shortSearch;
        private readonly ICommonExcelExporter _commonExcelExporter;
        private readonly IDatabaseRepository _customDatabaseRepository;
        private readonly IBrokerRepository _customBrokerRepository;



        public MasterLoLsAppService(IRepository<MasterLoL> masterLoLRepository,
            IMasterLolListofListsRepository customMasterLoLRepository,
            IRepository<Database, int> lookup_databaseRepository,
            AppSession mySession,
            IRepository<ListCASContact, int> listCasContactRepository,
            IRepository<ListMailer, int> listMailerRepository,
            IRepository<ListMailerRequested, int> listReqMailerRepository,
            IShortSearch shortSearch,
            IDatabaseRepository customDatabaseRepository,
             ICommonExcelExporter commonExcelExporter,
                IBrokerRepository customBrokerRepository,
                 IRepository<BuildLol, int> buildLolRepository)
        {
            _customMasterLoLRepository = customMasterLoLRepository;
            _lookup_databaseRepository = lookup_databaseRepository;
            _mySession = mySession;
            _listMailerRepository = listMailerRepository;
            _listCasContactRepository = listCasContactRepository;
            _listReqMailerRepository = listReqMailerRepository;
            _shortSearch = shortSearch;
            _commonExcelExporter = commonExcelExporter;
            _customDatabaseRepository = customDatabaseRepository;
            _customBrokerRepository = customBrokerRepository;
            _buildLolRepository = buildLolRepository;
        }

        public PagedResultDto<MasterLoLForViewDto> GetAll(GetAllMasterLoLsInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var shortWhere = _shortSearch.GetWhere(PageID.ListOfList, input.Filter);
                var query = GetAllListsofListQuery(input, shortWhere);
                return _customMasterLoLRepository.GetAllListsOfList(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
         public List<DropdownOutputDto> GetAllOwners(int databaseId)
        {
            try
            {
                var query = GetallOwner(databaseId);
                return _customMasterLoLRepository.GetAllOwnersforlistoflist(query.Item1, query.Item2).OrderBy(x => x.Label).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<DropdownOutputDto> GetAllManagers(int databaseId)
        {
            try
            {
                var query = GetallManagers(databaseId);
                return _customMasterLoLRepository.GetAllManagersforlistoflist(query.Item1,query.Item2).OrderBy(x => x.Label).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public PagedResultDto<ContactTableDto> GetAllContacts(int ownerid, int managerid)
        {
            try
            {
                var query = GetallSendOrderToAndDwapContacts(ownerid, managerid);
                return _customMasterLoLRepository.GetallContacts(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<DropdownOutputDto> GetAllReqMailers(int databaseId)
        {
            try
            {
                var query = GetallMailers(databaseId);
                return _customMasterLoLRepository.GetAllMailersforlistoflist(query.Item1,query.Item2).OrderBy(x => x.Label).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<DropdownOutputDto> GetAllAvailableMailersForDropdown(int databaseId)
        {
            try
            {
                var query = GetAllAvailableMailers(databaseId);
                return _customMasterLoLRepository.GetAllAvailableMailersforlistoflist(query.Item1,query.Item2).OrderBy(x => x.Label).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public PagedResultDto<LookupForListofListDto> GetallDropdownsfromLookup()
        {
            try
            {
                var query = GetdropdownFromLookupTable();
                return _customMasterLoLRepository.GetallDropdownsfromLookups(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task<int> CreateOrEdit(CreateOrEditMasterLoLDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                
                if (input.Id == null)
                {
                    ValidateListName(input);
                    input.cCreatedBy = _mySession.IDMSUserName;
                input.dCreatedDate = DateTime.Now;
                var masterData = ObjectMapper.Map<MasterLoL>(input);
                var ListId = await _customMasterLoLRepository.InsertAndGetIdAsync(masterData);
                CurrentUnitOfWork.SaveChanges();
                return ListId;
                }
                else
                {
                    var updatelist = _customMasterLoLRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updatelist);
                    CurrentUnitOfWork.SaveChanges();
                    return updatelist.Id;
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        
        public CreateOrEditMasterLoLDto GetListById(int Id,int databaseId)
        {
            try
            {
                var query = GetListsById(Id,databaseId);
                var editData = _customMasterLoLRepository.GetListsofListById(query);
                var data= ObjectMapper.Map<CreateOrEditMasterLoLDto>(editData);
                data.listofContacts = FetchCASContact(Id).Items;
                data.listOfMailers = FetchMailers(Id).Items;
                data.listofReqMailer = FetchReqMailers(Id).Items;
                return data;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public PagedResultDto<CreateOrEditListMailerDto> FetchMailers(int Id)
        {
            var query = GetMailers(Id);
            return _customMasterLoLRepository.GetAvailableMailersById(query);
        }
        public PagedResultDto<CreateOrEditListCASContacts> FetchCASContact(int Id)
        {
            var query = GetContacts(Id);
            return _customMasterLoLRepository.GetCASContactsById(query);
             
        }
        public PagedResultDto<CreateOrEditListMailerRequestedDto> FetchReqMailers(int Id)
        {
            var query = GetListReqMailer(Id);
            return _customMasterLoLRepository.GetReqMailersById(query);          
        }
        public async Task CreateOrEditForDwapContacts(List<int>dwapContacts,int listId)
        {
            try
            {
                var dwapdata = FetchCASContact(listId).Items;
                
                    if (dwapdata.Count > 0)
                    {
                       for(int i=0; i < dwapdata.Count; i++)
                        {
                            int Id = Convert.ToInt32(dwapdata[i].Id);
                            _listCasContactRepository.DeleteAsync(Id);
                        }
                    }
                    for (int i = 0; i < dwapContacts.Count; i++)
                    {
                        var dwapObject = new ListCASDto
                        {
                            ListID = listId,
                            ContactID = dwapContacts[i],
                            cCreatedBy = _mySession.IDMSUserName,
                            dCreatedDate = DateTime.Now,

                        };
                        var dwapContact = ObjectMapper.Map<ListCASContact>(dwapObject);
                        await _listCasContactRepository.InsertAsync(dwapContact);
                        CurrentUnitOfWork.SaveChanges();

                    }

                


            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task CreateOrEditForRequestedMailer(List<int> requestedByMailer,int listId)
        {
            try
            {
                var reqData = FetchReqMailers(listId).Items;
                if(reqData.Count> 0)
                {
                    for (int i = 0; i < reqData.Count; i++)
                    {
                        int Id = Convert.ToInt32(reqData[i].Id);
                        _listReqMailerRepository.DeleteAsync(Id);
                    }

                }
               
                    for (int i = 0; i < requestedByMailer.Count; i++)
                    {
                        var reqMailerObj = new CreateOrEditListMailerRequestedDto
                        {
                            ListID = listId,
                            MailerID = requestedByMailer[i],
                            cCreatedBy = _mySession.IDMSUserName,
                            dCreatedDate = DateTime.Now,

                        };
                        var reqMailer = ObjectMapper.Map<ListMailerRequested>(reqMailerObj);
                        await _listReqMailerRepository.InsertAsync(reqMailer);
                        CurrentUnitOfWork.SaveChanges();

                    }
                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        
        }

        public async Task CreateOrEditForAvailableMailer(List<int> availableToMailer, int listId)
        {
            try
            {
                var availabledata = FetchMailers(listId).Items;
                if (availabledata.Count > 0)
                {
                    for (int i = 0; i < availabledata.Count; i++)
                    {
                        int Id = Convert.ToInt32(availabledata[i].Id);
                        _listCasContactRepository.DeleteAsync(Id);
                    }

                }

                for (int i = 0; i < availableToMailer.Count; i++)
                    {
                        var listmailerObj = new CreateOrEditListMailerDto
                        {
                            ListID = listId,
                            MailerID = availableToMailer[i],
                            cCreatedBy = _mySession.IDMSUserName,
                            dCreatedDate = DateTime.Now,

                        };
                        var dwapContact = ObjectMapper.Map<ListMailer>(listmailerObj);
                        await _listMailerRepository.InsertAsync(dwapContact);
                        CurrentUnitOfWork.SaveChanges();

                    }
                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public void ValidateListName(CreateOrEditMasterLoLDto input)
        {

            var query = GetAllLists(input.DatabaseId);
            var datafor = _customMasterLoLRepository.GetAllListsName(query);
            for (int i = 0; i < datafor.Items.Count; i++)
            {
                if (input.cListName == datafor.Items[i].cListName)
                {
                    throw new UserFriendlyException("List Name must be Unique");
                }
            }


        }
        public IEnumerable<ExportToExcelMasterLolDto> GetDataExportAllMailerAccessToExcel(GetAllForTableInput input)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var query = GetAllListForExportToExcel(input);
               var listsdwap=  _customMasterLoLRepository.GetallDataForExportToMaierAccess(query);
                sw.Stop();
                Logger.Info($"\r\n ----- Total time for FetchDecoys: {sw.Elapsed.TotalSeconds} ----- \r\n");
                return listsdwap;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        
       
        public List<ExportListMailerAccess> GetExportAllListMailerAccessToExcel(GetAllForTableInput input)
        {
            try
            {
                var query = GetAllListMailerAccessForExportToExcel(input);
                return _customMasterLoLRepository.GetallDataForExportToListMaierAccess(query);


            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public FileDto ExportAllListMailerAccessToExcel(GetAllForTableInput input)
        {
            try
            {
                var excelData = GetExportAllListMailerAccessToExcel(input);
                var databaseName = _customDatabaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseNameForExcel")}{databaseName}";
                var fileName = $"ListMailerAccess";

                return _commonExcelExporter.AllExportToFileForListMailerAccess(excelData, databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public FileDto ExportAllMailerAccessToExcel(GetAllForTableInput input)
        {
            try
            {
                var excelData = GetDataExportAllMailerAccessToExcel(input).ToList();
            
                var databaseName = _customDatabaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseNameForExcel")}{databaseName}";
                var fileName = $"MasterLists";

                return _commonExcelExporter.AllExportToFileForMasterLol(excelData, databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        private Tuple<string, string, List<SqlParameter>> GetAllLists(int databaseId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"M.cListName");
            query.AddFrom("TBLMASTERLOL", "M");
            query.AddWhere("", "DATABASEID", "EQUALTO", databaseId.ToString());
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        #region MasterLolBizness

        private Tuple<string, string, List<SqlParameter>> GetMailers(int Id)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"M.ID,M.ListID,M.MailerID,M.cCreatedBy,M.dCreatedDate");
            query.AddFrom("tblListMailer", "M");
            query.AddWhere("", "M.ListID", "EQUALTO", Id.ToString());
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        private string GetAllListMailerAccessForExportToExcel(GetAllForTableInput input)
        {
            var builder = new StringBuilder();
            builder.Append($"SELECT M.ListID as [List ID], MLOL.cCode as [Code], MLOL.cListName as [List Name],MLOL.LK_PermissionType as [Type],ML.cCompany + ' (' + convert(varchar, M.mailerID) + ')' as Company");
            builder.Append(" FROM TBLMASTERLOL AS MLOL ");
            builder.Append(" INNER JOIN TBLLISTMAILER AS M ON MLOL.ID = M.LISTID ");
            builder.Append(" LEFT OUTER JOIN TBLMANAGER AS MGR ON MLOL.MANAGERID = MGR.ID ");
            builder.Append(" LEFT OUTER JOIN TBLMAILER AS ML ON M.MAILERID = ML.ID ");
            builder.Append($" WHERE ML.BrokerID IN (SELECT ID FROM tblBroker where databaseid = {input.SelectedDatabase})");
            builder.Append(" AND MLOL.ID IN (SELECT MasterLOLID from tblBuildLoL)");
            builder.Append(" AND MLOL.LK_PermissionType IN ('H', 'P')");
            return builder.ToString();
                   }
        private string GetAllListForExportToExcel(GetAllForTableInput input)
        {
            var builder = new StringBuilder();
            builder.Append($"SELECT MLOL.ID,MLOL.CLISTNAME,MGR.CCOMPANY AS [MANAGERNAME], c.cDescription as DecisionGroup, LO.CCOMPANY AS [LISTOWNER], MLOL.LK_PermissionType,a.cDescription as ListType, b.cDescription as productcode,MLOL.cCode, MLOL.cMINDatacardCode, MLOL.cNextMarkID, MLOL.nbaseprice_postal, MLOL.nbaseprice_telemarketing, MLOL.cAppearDate, MLOL.cLastUpdateDate, MLOL.cRemoveDate,CON.cFirstName + ' ' + CON.cLastName + '(' + CON.cEmailAddress + ')' as sendorderto,MLOL.cCustomText1, MLOL.cCustomText2, MLOL.cCustomText3, MLOL.cCustomText4, MLOL.cCustomText5, MLOL.cCustomText6, MLOL.cCustomText7, MLOL.cCustomText8, MLOL.cCustomText9, MLOL.cCustomText10,"
                + "MLOL.LK_DECISIONGROUP, MLOL.LK_LISTTYPE, MLOL.LK_PRODUCTCODE, MLOL.OWNERID , case when IISMULTIBUYER = 1 then 'Yes' else 'No' end as multibuyer," +
                " MLOL.LK_PermissionType AS TYPEOFLIST, case when iDropDuplicates = 1 then 'Yes' else 'No' end as DD, case when iSendCASApproval = 1 then 'Yes' else 'No' end as SendDWAP, case when mlol.iIsActive = 1 then 'Yes' else 'No' end as Active");
            builder.Append("  FROM TBLMASTERLOL AS MLOL  ");
            builder.Append("LEFT OUTER JOIN TBLMANAGER AS MGR ON MLOL.MANAGERID = MGR.ID  ");
            builder.Append("LEFT OUTER JOIN TBLOWNER AS LO ON MLOL.OWNERID = LO.ID  ");
            builder.Append("LEFT OUTER JOIN TBLLOOKUP AS a ON MLOL.LK_ListType = a.cCode  ");
            builder.Append("AND a.cLookupValue='LISTTYPE'  ");
            builder.Append("LEFT OUTER JOIN TBLLOOKUP AS b ON MLOL.LK_ProductCode= b.cCode  ");
            builder.Append("AND b.cLookupValue='PRODUCTCODE'  ");
            builder.Append("LEFT OUTER JOIN TBLLOOKUP AS c ON MLOL.LK_DecisionGroup = c.cCode  ");
            builder.Append("AND c.cLookupValue='DECISIONGROUP' ");
            builder.Append("INNER JOIN TBLCONTACT AS CON ON MLOL.iOrderContactID = CON.ID  ");
            builder.Append($"WHERE MLOL.DATABASEID= {input.SelectedDatabase}  ");
            builder.Append("ORDER BY MLoL.cListName ASC  ");
            builder.Append($"Select lc.ListID, c.cFirstName + ' ' + c.cLastName as DWAPContact, c.cemailAddress   ");
            builder.Append("FROM tblListCASContact AS lc  ");
            builder.Append("INNER JOIN tblContact AS c on lc.ContactID = c.ID   ");
            builder.Append($"where ListID in(select mlol.id from tblMasterLoL mlol LEFT OUTER JOIN TBLMANAGER MGR ON MLOL.MANAGERID = MGR.ID  Where MLOL.DatabaseID ={input.SelectedDatabase}   )");
            
            builder.Append($"SELECT L.ListID, m.cCompany  ");
            builder.Append("FROM tblListMailerRequested AS L  ");
            builder.Append("INNER JOIN tblMailer AS M on L.MailerID = M.ID  ");
            builder.Append($"where ListID in(select mlol.id from tblMasterLoL mlol LEFT OUTER JOIN TBLMANAGER MGR ON MLOL.MANAGERID = MGR.ID  Where MLOL.DatabaseID = {input.SelectedDatabase} )  ");
            builder.Append($"select LM.ListID, m.cCompany  ");
            builder.Append("FROM tblListMaiLER AS LM  ");
            builder.Append("INNER JOIN tblMailer AS M on LM.MailerID = M.ID  ");
            builder.Append($"where ListID in(select mlol.id from tblMasterLoL mlol LEFT OUTER JOIN TBLMANAGER MGR ON MLOL.MANAGERID = MGR.ID  Where MLOL.DatabaseID = {input.SelectedDatabase} )");


            return builder.ToString();
        }
 
        private Tuple<string, string, List<SqlParameter>> GetContacts(int Id)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"C.ID,C.ListID,C.ContactID,C.cCreatedBy,C.dCreatedDate");
            query.AddFrom("tblListCASContact", "C");
            query.AddWhere("", "C.ListID", "EQUALTO", Id.ToString());
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        private Tuple<string, string, List<SqlParameter>> GetListReqMailer(int Id)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"R.ID,R.ListID,R.MailerID,R.cCreatedBy,R.dCreatedDate");
            query.AddFrom("tblListMailerRequested", "R");
            query.AddWhere("", "R.ListID", "EQUALTO", Id.ToString());
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        private Tuple<string, string, List<SqlParameter>> GetdropdownFromLookupTable()
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"l.ID,l.cLookupValue,l.cCode, l.cDescription,l.iOrderBy,l.cField,l.mField,l.iField,l.iIsActive,l.cCreatedBy,l.dCreatedDate");
            query.AddFrom("tblLookup", "l");
            query.AddWhere("", "l.iIsActive", "EQUALTO", "1");
            query.AddWhere("AND", "l.cLookupValue", "EQUALTO", "DECISIONGROUP");
            query.AddWhere("OR", "l.cLookupValue", "EQUALTO", "PRODUCTCODE");
            query.AddWhere("OR", "l.cLookupValue", "EQUALTO", "LISTTYPE");
            query.AddSort("l.cLookupValue ASC");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        private Tuple<string, List<SqlParameter>> GetallOwner(int databaseId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"O.ID, O.CCODE, O.CCOMPANY, O.CCity, O.cAddress1, O.cAddress2, O.CSTATE, O.cPhone, O.cFax, O.cZip, (SELECT COUNT(*) FROM TBLCONTACT CCOUNT WHERE CCOUNT.CONTACTID = O.ID AND CCOUNT.CTYPE = {Convert.ToInt32(ContactType.Owner).ToString()}) AS contactsCount,STUFF(ISNULL(',' + nullif(O.CCity,''), '') + ISNULL(',' + nullif( O.CSTATE,''), '') + ISNULL(' ' + nullif(O.CZIP, ''),''),1,1,'') AS Address,O.IISACTIVE");
            query.AddFrom("TBLOWNER", "O");
            query.AddJoin("TBLCONTACT", "C", "ID", "O", "LEFT JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Owner).ToString());
            query.AddWhere("", "O.DatabaseID", "EQUALTO", databaseId.ToString());
            query.AddWhere("AND", "O.iIsActive", "EQUALTO", "1");
            query.AddSort("O.CCOMPANY ASC");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sqlSelect.ToString(), sqlParams);
        }
        private Tuple<string,List<SqlParameter>> GetallManagers(int databaseId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"O.ID, O.CCODE, O.CCOMPANY, O.CCity, O.cAddress1, O.cAddress2, O.CSTATE, O.cPhone, O.cFax, O.cZip, (SELECT COUNT(*) FROM TBLCONTACT CCOUNT WHERE CCOUNT.CONTACTID = O.ID AND CCOUNT.CTYPE = {Convert.ToInt32(ContactType.Owner).ToString()} AND CCOUNT.iIsActive='1') AS contactsCount,STUFF(ISNULL(',' + nullif(O.CCity,''), '') + ISNULL(',' + nullif( O.CSTATE,''), '') + ISNULL(' ' + nullif(O.CZIP, ''),''),1,1,'') AS Address,O.IISACTIVE");
            query.AddFrom("TBLMANAGER", "O");
            query.AddJoin("TBLCONTACT", "C", "ID", "O", "LEFT JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Owner).ToString());
            query.AddWhere("", "O.DatabaseID", "EQUALTO", databaseId.ToString());
            query.AddWhere("AND", "O.iIsActive", "EQUALTO", "1");
            query.AddSort("O.CCOMPANY ASC");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string,List<SqlParameter>>(sqlSelect.ToString(),sqlParams);
        }
        private Tuple<string, string, List<SqlParameter>> GetAllListsofListQuery(GetAllMasterLoLsInput filters,string shortWhere)
        {
            try
            {
                var query = new Common.QueryBuilder();
                query.AddSelect("MLOL.ID, MLOL.CLISTNAME,L.cDescription,MLOL.CCODE, MLOL.cNextMarkID,MLOL.OWNERID,LO.CCOMPANY AS [LISTOWNER],MLOL.IISMULTIBUYER, MGR.CCOMPANY AS[MANAGERNAME],MLOL.iIsActive,MLOL.LK_PermissionType,MLOL.LK_DECISIONGROUP,MLOL.LK_ListType,MLOL.LK_PRODUCTCODE");
                query.AddFrom("TBLMASTERLOL", "MLOL");
                query.AddJoin("TBLOWNER", "LO", "OWNERID", "MLOL", "LEFT JOIN", "ID");
                query.AddJoin("TBLMANAGER", "MGR", "MANAGERID", "MLOL", "INNER JOIN", "ID");
                query.AddJoin("TBLLOOKUP", "L", "LK_PermissionType", "MLOL", "INNER JOIN", "cCode");
                query.AddJoin("TBLCONTACT", "C", "iOrderContactID", "MLOL", "LEFT JOIN", "ID").And("C.CTYPE", "EQUALTO", "3").And("c.ContactID", "EQUALTO", "MGR.ID").And("C.iIsActive", "EQUALTO", "1");
                query.AddWhere("AND","L.cLookupValue", "EQUALTO", "PERMISSIONTYPE");
                query.AddWhere("AND", "MLOL.DatabaseID", "EQUALTO", filters.DatabaseID);
                if (shortWhere.Length > 0)
                    query.AddWhereString($"AND ({shortWhere})");
                query.AddSort(filters.Sorting ?? "MLOL.ID DESC");
                query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
                query.AddDistinct();
                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

                var sqlCount = query.BuildCount().Item1;
                return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private Tuple<string,List<SqlParameter>> GetallMailers(int databaseId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"O.ID, O.CCODE, O.CCOMPANY,B.CCOMPANY,O.CCity, O.cAddress1, O.cAddress2, O.CSTATE, O.cPhone, O.cFax, O.cZip, (SELECT COUNT(*) FROM TBLCONTACT CCOUNT WHERE CCOUNT.CONTACTID = O.ID AND CCOUNT.CTYPE = {Convert.ToInt32(ContactType.Owner).ToString()} AND CCOUNT.iIsActive='1') AS contactsCount,STUFF(ISNULL(',' + nullif(O.CCity,''), '') + ISNULL(',' + nullif( O.CSTATE,''), '') + ISNULL(' ' + nullif(O.CZIP, ''),''),1,1,'') AS Address,O.IISACTIVE");
            query.AddFrom("TBLMAILER", "O");
            query.AddJoin("TBLCONTACT", "C", "ID", "O", "LEFT JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Owner).ToString());
            query.AddJoin("TBLBROKER", "B", "BROKERID", "O", "LEFT JOIN", "ID");
            query.AddWhere("", "O.DatabaseID", "EQUALTO", databaseId.ToString());
            query.AddWhere("AND", "O.iIsActive", "EQUALTO", "1");
            query.AddSort("O.CCOMPANY ASC");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string,List<SqlParameter>>(sqlSelect.ToString(),sqlParams);
        }
        private Tuple<string,List<SqlParameter>> GetAllAvailableMailers(int databaseId)
        {
            try
            {
                var query = new Common.QueryBuilder();
                query.AddSelect($"O.ID, O.CCODE, O.CCOMPANY,B.CCOMPANY,O.CCity, O.cAddress1, O.cAddress2, O.CSTATE, O.cPhone, O.cFax, O.cZip, (SELECT COUNT(*) FROM TBLCONTACT CCOUNT WHERE CCOUNT.CONTACTID = O.ID AND CCOUNT.CTYPE = {Convert.ToInt32(ContactType.Mailer).ToString()} AND CCOUNT.iIsActive='1') AS contactsCount,STUFF(ISNULL(',' + nullif(O.CCity,''), '') + ISNULL(',' + nullif( O.CSTATE,''), '') + ISNULL(' ' + nullif(O.CZIP, ''),''),1,1,'') AS Address,O.IISACTIVE");
                query.AddFrom("TBLMAILER", "O");
                query.AddJoin("TBLCONTACT", "C", "ID", "O", "LEFT JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Mailer).ToString());
                query.AddJoin("TBLBROKER", "B", "BROKERID", "O", "LEFT JOIN", "ID");
                query.AddWhere("", "O.DatabaseID", "EQUALTO", databaseId.ToString());
                query.AddWhere("AND", "O.iIsActive", "EQUALTO", "1");
                query.AddSort("O.CCOMPANY ASC");
                query.AddDistinct();

                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                return new Tuple<string,List<SqlParameter>>(sqlSelect.ToString(),sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private Tuple<string, string, List<SqlParameter>> GetallSendOrderToAndDwapContacts(int ownerId, int managerId)
        {
            var query = new Common.QueryBuilder();
            if (ownerId > 0 && managerId > 0)
            {

                query.AddSelect($"c.ID, c.cLastName + ',' +c.cFirstName as Name , cEMailAddress");
                query.AddFrom("tblContact", "c");
                query.AddWhere("", "c.iIsActive", "EQUALTO", "1");
                query.AddWhere("AND", "c.cType", "EQUALTO", Convert.ToInt32(ContactType.Manager).ToString());
                query.AddWhere("AND", "c.CONTACTID", "EQUALTO", managerId.ToString());
                query.AddWhere("OR", "c.iIsActive", "EQUALTO", "1");
                query.AddWhere("AND", "c.cType", "EQUALTO", Convert.ToInt32(ContactType.Owner).ToString());
                query.AddWhere("AND", "c.CONTACTID", "EQUALTO", ownerId.ToString());

            }
            if (ownerId > 0 && managerId == 0)
            {
                query.AddSelect($"c.ID, c.cLastName + ',' +c.cFirstName as Name , cEMailAddress");
                query.AddFrom("tblContact", "c");
                query.AddWhere("", "c.iIsActive", "EQUALTO", "1");
                query.AddWhere("AND", "c.cType", "EQUALTO", Convert.ToInt32(ContactType.Owner).ToString());
                query.AddWhere("AND", "c.CONTACTID", "EQUALTO", ownerId.ToString());
            }
            if (managerId > 0 && ownerId == 0)
            {
                query.AddSelect($"c.ID, c.cLastName + ',' +c.cFirstName as Name , cEMailAddress");
                query.AddFrom("tblContact", "c");
                query.AddWhere("", "c.iIsActive", "EQUALTO", "1");
                query.AddWhere("AND", "c.cType", "EQUALTO", Convert.ToInt32(ContactType.Manager).ToString());
                query.AddWhere("AND", "c.CONTACTID", "EQUALTO", managerId.ToString());
            }

            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        
            private Tuple<string, string, List<SqlParameter>> GetListsById(int ListId,int databaseId)
            {
            try
            {
                var query = new Common.QueryBuilder();
                query.AddSelect("MLOL.ID,MLOL.DatabaseID ,MLOL.dCreatedDate,MLOL.cCreatedBy,MLOL.CLISTNAME,MLOL.iSendCASApproval ,MLOL.CCODE,MLOL.CMinDatacardCode ,MLOL.cNextMarkID,MLOL.OWNERID,MLOL.MANAGERID,MLOL.IISMULTIBUYER, MLOL.iOrderContactID,MLOL.nBasePrice_Postal,MLOL.nBasePrice_Telemarketing,MLOL.iIsActive,MLOL.iDropDuplicates,MLOL.cCustomText1,MLOL.cCustomText2,MLOL.cCustomText3,MLOL.cCustomText4,MLOL.cCustomText5,MLOL.cCustomText6,MLOL.cCustomText7,MLOL.cCustomText8,MLOL.cCustomText9,MLOL.cCustomText10,MLOL.iIsActive,MLOL.LK_PermissionType,MLOL.LK_DECISIONGROUP,MLOL.LK_ListType,MLOL.LK_PRODUCTCODE,MLOL.cAppearDate,MLOL.CRemoveDate,MLOL.cLastUpdateDate,MLOL.iIsProfanityCheckRequired,MLOL.iIsNCOARequired");
                query.AddFrom("TBLMASTERLOL", "MLOL");
                query.AddJoin("TBLOWNER", "LO", "OWNERID", "MLOL", "LEFT JOIN", "ID");
                query.AddJoin("TBLMANAGER", "MGR", "MANAGERID", "MLOL", "INNER JOIN", "ID");
                query.AddJoin("TBLCONTACT", "C", "iOrderContactID", "MLOL", "LEFT JOIN", "ID").And("C.CTYPE", "EQUALTO", "3").And("c.ContactID", "EQUALTO", "MGR.ID").And("C.iIsActive", "EQUALTO", "1");
                query.AddWhere("AND", "MLOL.DatabaseID", "EQUALTO",databaseId.ToString());
                query.AddWhere("AND", "MLOL.ID", "EQUALTO", ListId.ToString());
                query.AddSort("MLOL.ID DESC");
                //query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
                query.AddDistinct();
                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                //sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

                var sqlCount = query.BuildCount().Item1;
                return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #endregion
    }
}