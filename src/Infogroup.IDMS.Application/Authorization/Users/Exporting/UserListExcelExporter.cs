using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.Authorization.Users.Dto;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.Authorization.Users.Exporting
{
    public class UserListExcelExporter : EpPlusExcelExporterBase, IUserListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UserListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<UserListDto> userListDtos)
        {
            return CreateExcelPackage(
                "UserList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Users"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Surname"),
                        L("UserName"),
                        L("PhoneNumber"),
                        L("EmailAddress"),
                        L("EmailConfirm"),
                        L("Roles"),
                        L("Active"),
                        L("CreationTime"),
                        L("DivisionCode"),
                        L("Permissions")
                        );

                    AddObjects(
                        sheet, 2, userListDtos,
                        _ => _.Name,
                        _ => _.Surname,
                        _ => _.UserName,
                        _ => _.PhoneNumber,
                        _ => _.EmailAddress,
                        _ => _.IsEmailConfirmed,
                        _ => _.Roles.Select(r => r.RoleName).JoinAsString(", "),
                        _ => _.IsActive,
                        _ => _timeZoneConverter.Convert(_.CreationTime, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.DivisionCode,
                        _ => _.GrantedPermissions
                        );

                    //Formatting cells

                    var creationTimeColumn = sheet.Column(9);
                    creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 11; i++)
                    {
                        sheet.Column(i).AutoFit();
                        sheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    }
                    for (int i = 2; i < userListDtos.Count + 2; i++)
                    {
                        sheet.Row(i).Height = 30;
                    }
                });
        }
    }
}
