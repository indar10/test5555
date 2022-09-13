using Infogroup.IDMS.Databases;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Decoys.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Mailers.Dtos;
using Abp.UI;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Common;
using Syncfusion.XlsIO;
using Abp.AspNetZeroCore.Net;
using System.IO;
using Abp.Authorization;
using Infogroup.IDMS.Authorization;

namespace Infogroup.IDMS.Decoys
{
    [AbpAuthorize(AppPermissions.Pages_Decoys)]
    public class DecoysAppService : IDMSAppServiceBase, IDecoysAppService
    {
        private readonly IDecoyRepository _customDecoyRepository;
        private readonly AppSession _mySession;
        private readonly IShortSearch _shortSearch;
        private readonly IRepository<Database, int> _databaseRepository;

        public DecoysAppService(
            IDecoyRepository customDecoyRepository, 
            AppSession mySession,
            IShortSearch shortSearch,
            IRepository<Database, int> databaseRepository
            )
        {
            _customDecoyRepository = customDecoyRepository;
            _mySession = mySession;
            _shortSearch = shortSearch;
            _databaseRepository = databaseRepository;
        }

        public PagedResultDto<MailerDto> GetAllDecoyMailers(GetAllDecoysInput input)
        {
            try
            {
                var shortWhere = _shortSearch.GetWhere(PageID.Decoys, input.Filter);                
                return _customDecoyRepository.GetAllDecoyMailers(_mySession.IDMSUserId, input, shortWhere); 
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }          
        }

        public PagedResultDto<DecoyDto> GetDecoysByMailer(GetAllDecoysInput input)
        {
            try
            {
                var shortWhere = _shortSearch.GetWhere(PageID.Decoys, input.Filter);               
                return _customDecoyRepository.GetDecoysByMailer(input, shortWhere);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CreateOrEditDecoyDto> GetDecoyForEdit(EntityDto input)
        {
            try
            {
                var decoy = await _customDecoyRepository.FirstOrDefaultAsync(input.Id);
                return CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditDecoyDto>(decoy));
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }           
        }

        public List<DropdownOutputDto> FillGroupsForEdit()
        {
            try
            {
                var listOfGroupsKey = new List<DropdownOutputDto>();
                for (int i = 65; i < 91; i++)
                {
                    var key = Convert.ToChar(i).ToString();
                    listOfGroupsKey.Add(new DropdownOutputDto { Value = key, Label = key }
                    );
                }
                return listOfGroupsKey;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }           
        }

        [AbpAuthorize(AppPermissions.Pages_Decoys_Create, AppPermissions.Pages_Decoys_Edit)]
        public async Task CreateOrEditDecoy(CreateOrEditDecoyDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                input.cName = $"{input.cFirstName} {input.cLastName}";
                if (input.Id == null)
                {
                    input.cDecoyType = "M";
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var decoy = ObjectMapper.Map<Decoy>(input);
                    await _customDecoyRepository.InsertAsync(decoy);
                }
                else
                {
                    var updateDecoy = _customDecoyRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateDecoy);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }            
        }

        [AbpAuthorize(AppPermissions.Pages_Decoys_Create)]
        public void CopyDecoy(int input)
        {
            try
            {
                _customDecoyRepository.CopyDecoy(input, _mySession.IDMSUserName);                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }          
        }

        [AbpAuthorize(AppPermissions.Pages_Decoys_Delete)]
        public async Task DeleteDecoy(int input)
        {
            try
            {
                await _customDecoyRepository.DeleteAsync(input);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Decoys_Print)]
        public FileDto ExportToExcel(GetAllDecoysInput input)
        {
            try
            {                
                var decoyMailersList = GetAllDecoyMailers(input);
                var excelData = decoyMailersList.Items.ToList().Select(decoy =>
                {
                    input.mailerId = decoy.Id;
                    var decoyExcelDto = ObjectMapper.Map<DecoyExcelExporterDto>(decoy);
                    decoyExcelDto.DecoysList = GetDecoysByMailer(input).Items.ToList();
                    return decoyExcelDto;
                }).ToList();

                var databaseName = _databaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseName")}:{databaseName}";
                var fileName = $"{L("DecoyExcel")}";

                return AllDecoyExportToFile(excelData, databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private FileDto AllDecoyExportToFile(List<DecoyExcelExporterDto> excelDetails, string databaseName, string fileName)
        {
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Excel2016;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);                   
                    IWorksheet worksheet = workbook.Worksheets[0];
                    worksheet.Name = "SEED";

                    //Disable gridlines in the worksheet
                    worksheet.IsGridLinesVisible = false;

                    //Enter values to the cells from A1
                    worksheet.Range["A1"].Text = databaseName;

                    //Header value
                    worksheet.Range["A3"].Text = "Company";
                    worksheet.Range["B3"].Text = "Address1";
                    worksheet.Range["C3"].Text = "Address2";
                    worksheet.Range["D3"].Text = "City";
                    worksheet.Range["E3"].Text = "State";
                    worksheet.Range["F3"].Text = "Zip";
                    worksheet.Range["G3"].Text = "Phone";

                    worksheet.Range["A3:G3"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                    //Loop for display data
                    var rowIndex = 5;
                    foreach (var item in excelDetails)
                    {
                        worksheet.Range[$@"A{rowIndex}"].Text = item.cCompany;
                        worksheet.Range[$@"B{rowIndex}"].Text = item.cAddress1;
                        worksheet.Range[$@"C{rowIndex}"].Text = item.cAddress2;
                        worksheet.Range[$@"D{rowIndex}"].Text = item.cCity;
                        worksheet.Range[$@"E{rowIndex}"].Text = item.cState;
                        if (string.IsNullOrWhiteSpace(item.cZip))
                            worksheet.Range[$@"F{rowIndex}"].Text = item.cZip;

                        else
                            worksheet.Range[$@"F{rowIndex}"].Formula = "=\"" + item.cZip + "\"";                                

                        worksheet.Range[$@"G{rowIndex}"].Text = string.IsNullOrWhiteSpace(item.cPhone) ?
                                (string.IsNullOrWhiteSpace(item.cFax) ? string.Empty : $"F:{item.cFax}") :
                                (string.IsNullOrWhiteSpace(item.cFax) ? $"P:{item.cPhone}" : $"P:{item.cPhone} / F:{item.cFax}");

                        rowIndex +=2;
                        // Headers Decoy
                        worksheet.Range[$@"B{rowIndex}"].Text = "Group";
                        worksheet.Range[$@"C{rowIndex}"].Text = "Name";
                        worksheet.Range[$@"D{rowIndex}"].Text = "Address1";
                        worksheet.Range[$@"E{rowIndex}"].Text = "Address2";
                        worksheet.Range[$@"F{rowIndex}"].Text = "City";
                        worksheet.Range[$@"G{rowIndex}"].Text = "State";
                        worksheet.Range[$@"H{rowIndex}"].Text = "Zip";
                        worksheet.Range[$@"I{rowIndex}"].Text = "Phone";
                        worksheet.Range[$@"J{rowIndex}"].Text = "Email";
                        worksheet.Range[$@"B{rowIndex}:J{rowIndex}"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;


                        foreach (var decoy in item.DecoysList)
                        {
                            rowIndex++;

                            worksheet.Range[$@"B{rowIndex}"].Text = decoy.cDecoyGroup;
                            worksheet.Range[$@"C{rowIndex}"].Text = decoy.cName;
                            worksheet.Range[$@"D{rowIndex}"].Text = decoy.cAddress1;
                            worksheet.Range[$@"E{rowIndex}"].Text = decoy.cAddress2;
                            worksheet.Range[$@"F{rowIndex}"].Text = decoy.cCity;
                            worksheet.Range[$@"G{rowIndex}"].Text = decoy.cState;
                            if (string.IsNullOrWhiteSpace(decoy.cZip))
                                worksheet.Range[$@"H{rowIndex}"].Text = decoy.cZip;
                            else
                                worksheet.Range[$@"H{rowIndex}"].Formula = "=\"" + decoy.cZip.Trim() + "\"";

                             worksheet.Range[$@"I{rowIndex}"].Text = string.IsNullOrWhiteSpace(decoy.cPhone) ? 
                                (string.IsNullOrWhiteSpace(decoy.cFax) ? string.Empty : $"F:{decoy.cFax}") : 
                                (string.IsNullOrWhiteSpace(decoy.cFax) ? $"P:{decoy.cPhone}" : $"P:{decoy.cPhone} / F:{decoy.cFax}"); 
                            worksheet.Range[$@"J{rowIndex}"].Text = decoy.cEmail;                       
                        }
                        rowIndex += 2;
                    }

                    worksheet.Range[$@"A3:A{rowIndex}"].AutofitColumns();
                    worksheet.AutofitColumn(2);
                    worksheet.AutofitColumn(3);
                    worksheet.AutofitColumn(4);
                    worksheet.AutofitColumn(5);
                    worksheet.AutofitColumn(6);
                    worksheet.AutofitColumn(7);
                    worksheet.SetColumnWidth(8, 7);
                    worksheet.AutofitColumn(9);
                    worksheet.AutofitColumn(10);

                    worksheet.Range[$@"A2:J{rowIndex}"].BorderAround(ExcelLineStyle.Thin);

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
    }
}