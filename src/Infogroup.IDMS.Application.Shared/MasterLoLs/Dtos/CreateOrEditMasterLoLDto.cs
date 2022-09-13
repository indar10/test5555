
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Infogroup.IDMS.ListCASContacts.Dtos;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;

namespace Infogroup.IDMS.MasterLoLs.Dtos
{
    public class CreateOrEditMasterLoLDto : EntityDto<int?>
    {
        
        public int DatabaseId { get; set; }

        public int OwnerID { get; set; }

        public int ManagerID { get; set; }

        public IReadOnlyList<CreateOrEditListCASContacts> listofContacts { get; set; }
        public IReadOnlyList<CreateOrEditListMailerDto> listOfMailers { get; set; }
        public IReadOnlyList<CreateOrEditListMailerRequestedDto> listofReqMailer { get; set; }

        public string LK_PermissionType { get; set; }


        public string LK_ListType { get; set; }


        public string LK_ProductCode { get; set; }


        public string LK_DecisionGroup { get; set; }


        public string cCode { get; set; }

        public string cDMIDWListNumber { get; set; }


        public string cListName { get; set; }


        public string cMINDatacardCode { get; set; }


        public string cNextMarkID { get; set; }

        public int iOrderContactID { get; set; }

        public bool iIsMultibuyer { get; set; }


        public string cAppearDate { get; set; }


        public string cLastUpdateDate { get; set; }


        public string cRemoveDate { get; set; }

        public bool iSendCASApproval { get; set; }

        public DateTime? dEmailSentDate { get; set; }

        public int nBasePrice_Postal { get; set; }

        public int nBasePrice_Telemarketing { get; set; }

        public int? nNewBasePrice_Postal { get; set; }

        public int? nNewBasePrice_Telemarketing { get; set; }

        public string cCAS_Signature { get; set; }

        public string cCAS_IPAddress { get; set; }

        public string cCAS_ApprovedBy { get; set; }

        public bool iIsActive { get; set; }

        public bool? iApprovalForm { get; set; }

        public bool iDropDuplicates { get; set; }


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

        public DateTime dCreatedDate { get; set; }


        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

        public bool iIsNCOARequired { get; set; }

        public bool iIsProfanityCheckRequired { get; set; }

        public Guid? GUID { get; set; }

        public DateTime? dValidUpTill { get; set; }


    }
}