using Infogroup.IDMS.Lookups.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using Abp.Application.Services.Dto;
using System.Data.SqlClient;
using Abp.UI;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.MasterLoLs;
using Infogroup.IDMS.MasterLoLs.Dtos;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using Infogroup.IDMS.ListCASContacts.Dtos;
using System.Linq;

namespace Infogroup.IDMS.MasterLol
{
    public class MasterLolListofListsRepository : IDMSRepositoryBase<MasterLoL, int>, IMasterLolListofListsRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public MasterLolListofListsRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public PagedResultDto<MasterLoLForViewDto> GetAllListsOfList(Tuple<string, string, List<SqlParameter>> query)

        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<MasterLoLForViewDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var MasterLolData = new List<MasterLoLForViewDto>();

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            MasterLolData.Add(new MasterLoLForViewDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                cListName = dataReader["cListName"].ToString().Trim(),
                                cCode = dataReader["cCode"].ToString().Trim(),
                                cNextMarkID = dataReader["cNextMarkID"].ToString().Trim(),
                                OwnerID = Convert.ToInt32(dataReader["OwnerID"]),
                                ListOwner = dataReader["LISTOWNER"].ToString(),
                                iIsMultibuyer = Convert.ToBoolean(dataReader["iIsMultibuyer"]),
                                ManagerName = dataReader["MANAGERNAME"].ToString(),
                                LK_PermissionType = dataReader["LK_PermissionType"].ToString().Trim(),
                                iIsActive = Convert.ToBoolean(dataReader["iIsActive"]),
                                LK_DecisionGroup = dataReader["LK_DecisionGroup"].ToString().Trim(),
                                LK_ListType = dataReader["LK_ListType"].ToString().Trim(),
                                LK_ProductCode = dataReader["LK_PRODUCTCODE"].ToString().Trim(),
                                permissionTypeValue= dataReader["cDescription"].ToString().Trim()
                            });
                        }
                    }
                    result.Items = MasterLolData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<DropdownOutputDto> GetAllOwnersforlistoflist(string Query, List<SqlParameter> sqlParameters)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var Owners = new List<DropdownOutputDto>();
                var MasterLolData = new List<GetOwnerForListofListDto>();

                using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Owners.Add(new DropdownOutputDto { Label = dataReader["cCompany"].ToString(), Value = Convert.ToInt32(dataReader["ID"]) });

                        }
                    }
                }
                return Owners;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<DropdownOutputDto> GetAllMailersforlistoflist(string Query, List<SqlParameter> sqlParameters)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

  
                var reqMailersdata = new List<DropdownOutputDto>();

                using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            reqMailersdata.Add(new DropdownOutputDto { Label = dataReader["cCompany"].ToString(), Value = Convert.ToInt32(dataReader["ID"]) });
                        }
                    }
                   
                }
                return reqMailersdata;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public List<DropdownOutputDto> GetAllManagersforlistoflist(string Query, List<SqlParameter> sqlParameters)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

               var managers = new List<DropdownOutputDto>();

                using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            managers.Add(new DropdownOutputDto { Label = dataReader["cCompany"].ToString(), Value = Convert.ToInt32(dataReader["ID"]) });

                        }
                    }
                   
                }
                return managers;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public PagedResultDto<LookupForListofListDto> GetallDropdownsfromLookups(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<LookupForListofListDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var MasterLolData = new List<LookupForListofListDto>();

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            MasterLolData.Add(new LookupForListofListDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                cLookupValue = dataReader["cLookupValue"].ToString().Trim(),
                                iOrderBy = Convert.ToInt32(dataReader["iOrderBy"]),
                                cCode = dataReader["cCode"].ToString().Trim(),
                                cDescription = dataReader["cDescription"].ToString().Trim(),
                                cField = dataReader["cField"].ToString().Trim(),
                                mField = dataReader["mField"].ToString().Trim(),
                                iField = Convert.ToInt32(dataReader["iField"]),
                                iIsActive = Convert.ToBoolean(dataReader["iIsActive"])

                            });
                        }
                    }
                    result.Items = MasterLolData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public PagedResultDto<ContactTableDto> GetallContacts(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<ContactTableDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var contactData = new List<ContactTableDto>();

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            contactData.Add(new ContactTableDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                Name = dataReader["Name"].ToString().Trim(),
                                email = dataReader["cEMailAddress"].ToString().Trim()

                            });
                        }
                    }
                    result.Items = contactData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public List<DropdownOutputDto> GetAllAvailableMailersforlistoflist(string Query, List<SqlParameter> sqlParameters)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var availableMailerData = new List<DropdownOutputDto>();

                using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            availableMailerData.Add(new DropdownOutputDto { Label = dataReader["cCompany"].ToString(), Value = Convert.ToInt32(dataReader["ID"]) });

                        }
                    }                  
                }
                return availableMailerData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
      public CreateOrEditMasterLoLDto GetListsofListById(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new CreateOrEditMasterLoLDto();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    command.Parameters.Clear();
                }

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result = new CreateOrEditMasterLoLDto
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                DatabaseId = Convert.ToInt32(dataReader["DatabaseId"]),
                                cCode = dataReader["cCode"].ToString().Trim(),
                                cListName = dataReader["cListName"].ToString().Trim(),
                                LK_DecisionGroup = dataReader["LK_DecisionGroup"].ToString().Trim(),
                                LK_ListType = dataReader["LK_ListType"].ToString().Trim(),
                                LK_PermissionType = dataReader["LK_PermissionType"].ToString().Trim(),
                                LK_ProductCode = dataReader["LK_ProductCode"].ToString().Trim(),
                                cMINDatacardCode = dataReader["cMINDatacardCode"].ToString().Trim(),
                                cNextMarkID = dataReader["cNextMarkID"].ToString().Trim(),
                                OwnerID = Convert.ToInt32(dataReader["OwnerID"]),
                                ManagerID = Convert.ToInt32(dataReader["ManagerID"]),
                                nBasePrice_Postal = Convert.ToInt32(dataReader["nBasePrice_Postal"]),
                                nBasePrice_Telemarketing = Convert.ToInt32(dataReader["nBasePrice_Telemarketing"]),
                                iIsActive = Convert.ToBoolean(dataReader["iIsActive"]),
                                iDropDuplicates = Convert.ToBoolean(dataReader["iIsActive"]),
                                cCustomText1 = dataReader["cCustomText1"].ToString().Trim(),
                                cCustomText2 = dataReader["cCustomText2"].ToString().Trim(),
                                cCustomText3 = dataReader["cCustomText3"].ToString().Trim(),
                                cCustomText4 = dataReader["cCustomText4"].ToString().Trim(),
                                cCustomText5 = dataReader["cCustomText5"].ToString().Trim(),
                                cCustomText6 = dataReader["cCustomText6"].ToString().Trim(),
                                cCustomText7 = dataReader["cCustomText7"].ToString().Trim(),
                                cCustomText8 = dataReader["cCustomText8"].ToString().Trim(),
                                cCustomText9 = dataReader["cCustomText9"].ToString().Trim(),
                                cCustomText10 = dataReader["cCustomText10"].ToString().Trim(),
                                iIsNCOARequired = Convert.ToBoolean(dataReader["iIsNCOARequired"]),
                                iSendCASApproval = Convert.ToBoolean(dataReader["iSendCASApproval"]),
                                cAppearDate = dataReader["cAppearDate"].ToString(),
                                cRemoveDate = dataReader["cRemoveDate"].ToString(),
                                cLastUpdateDate = dataReader["cLastUpdateDate"].ToString(),
                                iIsProfanityCheckRequired = Convert.ToBoolean(dataReader["iIsProfanityCheckRequired"]),
                                cCreatedBy = dataReader["cCreatedBy"].ToString().Trim(),
                                dCreatedDate = Convert.ToDateTime(dataReader["dCreatedDate"]),
                                iOrderContactID = Convert.ToInt32(dataReader["iOrderContactID"]),
                            } ;
                        }
                    }
                    
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

      }
        public PagedResultDto<CreateOrEditListMailerDto> GetAvailableMailersById(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<CreateOrEditListMailerDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var availableData = new List<CreateOrEditListMailerDto>();

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            availableData.Add(new CreateOrEditListMailerDto
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                MailerID = Convert.ToInt32(dataReader["MailerID"]),
                                ListID = Convert.ToInt32(dataReader["ListID"]),
                                cCreatedBy = dataReader["cCreatedBy"].ToString().Trim(),
                                dCreatedDate = Convert.ToDateTime(dataReader["dCreatedDate"]),
                                Action= ActionType.None

                            });
                        }
                    }
                    result.Items = availableData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }


        }
        public PagedResultDto<CreateOrEditListMailerRequestedDto> GetReqMailersById(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<CreateOrEditListMailerRequestedDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var requestedData = new List<CreateOrEditListMailerRequestedDto>();

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            requestedData.Add(new CreateOrEditListMailerRequestedDto
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                MailerID = Convert.ToInt32(dataReader["MailerID"]),
                                ListID = Convert.ToInt32(dataReader["ListID"]),
                                cCreatedBy = dataReader["cCreatedBy"].ToString().Trim(),
                                dCreatedDate = Convert.ToDateTime(dataReader["dCreatedDate"]),
                                Action = ActionType.None

                            });
                        }
                    }
                    result.Items = requestedData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }


        }
        public PagedResultDto<CreateOrEditListCASContacts> GetCASContactsById(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<CreateOrEditListCASContacts>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var CASContactData = new List<CreateOrEditListCASContacts>();

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            CASContactData.Add(new CreateOrEditListCASContacts
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                ContactID = Convert.ToInt32(dataReader["ContactID"]),
                                ListID = Convert.ToInt32(dataReader["ListID"]),
                                cCreatedBy = dataReader["cCreatedBy"].ToString().Trim(),
                                dCreatedDate = Convert.ToDateTime(dataReader["dCreatedDate"]),
                                Action = ActionType.None

                            });
                        }
                    }
                    result.Items = CASContactData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        
         public PagedResultDto<CreateOrEditMasterLoLDto> GetAllListsName(Tuple<string, string, List<SqlParameter>> query)
                  {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<CreateOrEditMasterLoLDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var data = new List<CreateOrEditMasterLoLDto>();

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            data.Add(new CreateOrEditMasterLoLDto
                            {
                                cListName = dataReader["cListName"].ToString().Trim()

                            }); ;
                        }
                    }
                    result.Items = data;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public PagedResultDto<CreateOrEditMasterLoLDto> GetAllListOfListsForExportToExcel(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<CreateOrEditMasterLoLDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var data = new List<CreateOrEditMasterLoLDto>();

                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            data.Add(new CreateOrEditMasterLoLDto
                            {
                                cListName = dataReader["cListName"].ToString().Trim()

                            }); ;
                        }
                    }
                    result.Items = data;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public IEnumerable<ExportToExcelMasterLolDto> GetallDataForExportToMaierAccess(string query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<ExportToExcelMasterLolDto>();

                var MasterLolData = new Dictionary<int, ExportToExcelMasterLolDto>();

                using (var command = _databaseHelper.CreateCommand(query, CommandType.Text))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            MasterLolData.Add(Convert.ToInt32(dataReader["ID"]), new ExportToExcelMasterLolDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                cListName=dataReader["cListName"].ToString().Trim(),
                                ManagerName=dataReader["MANAGERNAME"].ToString().Trim(),
                                 DecisionGroup=dataReader["DecisionGroup"].ToString().Trim(),
                                ListOwner=dataReader["LISTOWNER"].ToString().Trim(),
                                LK_PermissionType=dataReader["LK_PermissionType"].ToString().Trim(),
                                ListType=dataReader["ListType"].ToString().Trim(),
                                productCode=dataReader["productCode"].ToString().Trim(),
                                cCode=dataReader["cCode"].ToString().Trim(),
                                cMINDatacardCode=dataReader["cMINDatacardCode"].ToString().Trim(),
                                cNextMarkID=dataReader["cNextMarkID"].ToString().Trim(),
                                nBasePrice_Postal = Convert.ToInt32(dataReader["nBasePrice_Postal"]),
                                nBasePrice_Telemarketing = Convert.ToInt32(dataReader["nBasePrice_Telemarketing"]),
                                cAppearDate= dataReader["cAppearDate"].ToString().Trim(),
                                cLastUpdateDate= dataReader["cLastUpdateDate"].ToString().Trim(),
                                cRemoveDate= dataReader["cRemoveDate"].ToString().Trim(),
                                SendOrderTo= dataReader["SendOrderTo"].ToString().Trim(),
                                cCustomText1= dataReader["cCustomText1"].ToString().Trim(),
                                cCustomText2= dataReader["cCustomText2"].ToString().Trim(),
                                cCustomText3 = dataReader["cCustomText3"].ToString().Trim(),
                                cCustomText4 = dataReader["cCustomText4"].ToString().Trim(),
                                cCustomText5 = dataReader["cCustomText5"].ToString().Trim(),
                                cCustomText6 = dataReader["cCustomText6"].ToString().Trim(),
                                cCustomText7 = dataReader["cCustomText7"].ToString().Trim(),
                                cCustomText8 = dataReader["cCustomText8"].ToString().Trim(),
                                cCustomText9 = dataReader["cCustomText9"].ToString().Trim(),
                                cCustomText10 = dataReader["cCustomText10"].ToString().Trim(),
                                LK_DecisionGroup= dataReader["LK_DecisionGroup"].ToString().Trim(),
                                LK_ListType= dataReader["LK_ListType"].ToString().Trim(),
                                LK_ProductCode= dataReader["LK_ProductCode"].ToString().Trim(),
                                MultiBuyer=dataReader["multibuyer"].ToString().Trim(),
                                Active=dataReader["Active"].ToString().Trim(),
                                dD=dataReader["DD"].ToString().Trim(),
                                sendDwap= dataReader["sendDwap"].ToString().Trim(),
                                typeOfList =dataReader["TYPEOFLIST"].ToString().Trim(),
                                DwapContacts = new List<string>(),
                                ReqMailer = new List<string>(),
                                AvailableMailer = new List<string>(),


                            });
                        }

                        dataReader.NextResult();

                        while(dataReader.Read())
                        {
                            MasterLolData[Convert.ToInt32(dataReader["ListID"])].DwapContacts.Add(dataReader["DWAPContact"].ToString().Trim());
                        }
                        dataReader.NextResult();

                        while (dataReader.Read())
                        {
                            MasterLolData[Convert.ToInt32(dataReader["ListID"])].ReqMailer.Add(dataReader["cCompany"].ToString().Trim());
                        }
                        dataReader.NextResult();

                        while (dataReader.Read())
                        {
                            MasterLolData[Convert.ToInt32(dataReader["ListID"])].AvailableMailer.Add(dataReader["cCompany"].ToString().Trim());
                        }


                    }

                }
                return MasterLolData.Select(item => item.Value);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<ExportListMailerAccess> GetallDataForExportToListMaierAccess(string query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var MasterLolData = new List<ExportListMailerAccess>();

                using (var command = _databaseHelper.CreateCommand(query, CommandType.Text))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            MasterLolData.Add(new ExportListMailerAccess
                            {
                                ListId = Convert.ToInt32(dataReader["List ID"]),
                                ListName = dataReader["List Name"].ToString().Trim(),
                                Code = dataReader["Code"].ToString().Trim(),
                                Type = dataReader["Type"].ToString().Trim(),
                                Company = dataReader["Company"].ToString().Trim()


                            });
                        }
                    }
                }
                return MasterLolData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
      
    }
}
