using Abp.Application.Services.Dto;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.MasterLoLs.Dtos
{
    public  class ExportToExcelMasterLolDto : PagedAndSortedResultRequestDto
    {
        public int ID { get; set; }
        public int ListId { get; set; }
        public string cListName { get; set; }
        public string ManagerName { get; set; }
        public string DecisionGroup { get; set; }
        public string ListOwner { get; set; }
        public string ListType { get; set; }
        public string LK_PermissionType { get; set; }
        public string productCode { get; set; }
        public string cCode { get; set; }
        public string cMINDatacardCode { get; set; }
        public string cNextMarkID { get; set; }
        public int nBasePrice_Postal { get; set; }

        public int nBasePrice_Telemarketing { get; set; }
        public string cAppearDate { get; set; }

        public string cLastUpdateDate { get; set; }

        public string cRemoveDate { get; set; }
        public string SendOrderTo { get; set; }
        public string dD { get; set; }
        public string sendDwap { get; set; }
        public string Active { get; set; }
        public string MultiBuyer { get; set; }
        public string typeOfList { get; set; }
        public string cCustomText1 { get; set; }
        public string cCustomText2 { get; set; }
        public string cCustomText3 { get; set; }
        public string cCustomText4 { get; set; }
        public string cCustomText5 { get; set; }
        public string cCustomText6 { get; set; }
        public string cCustomText7 { get; set; }
        public string cCustomText8 { get; set; }
        public string cCustomText9 { get; set; }
        public string cCustomText10 { get; set; }
        public string LK_ListType { get; set; }
        public string LK_ProductCode { get; set; }
        public string LK_DecisionGroup { get; set; }
        public string Company { get; set; }
        public int OwnerID { get; set; }
        public List<string> DwapContacts { get; set; }

        public List<string> ReqMailer { get; set; }
        public List<string> AvailableMailer { get; set; }







    }
}
