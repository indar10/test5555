
using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.OrderExportParts.Dtos;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class GetCampaignsOutputDto
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public string FTPSite { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string FTPPassword { get; set; }

        public string Type { get; set; }
        public string Layout { get; set; }
        public bool? IsHeaderRow { get; set; }
        public bool? IsDataFileOnly { get; set; }
        public bool? IsUnzipped { get; set; }
        public string Sort { get; set; }
        public string PGPKey { get; set; }
        public string ShipTo { get; set; }
        public string Email { get; set; }

        public string LayoutId { get; set; }

        public int iHasKeyCode { get; set; }
        public int TotalOutputQuantity { get; set; }

        public string Media { get; set; }
        public string FileLabel { get; set; }
        public string FileNotes { get; set; }

        public int? SplitType { get; set; }
        public int? SplitIntoNParts { get; set; }


        public List<DropdownOutputDto> TypeList { get; set; }
        public List<DropdownOutputDto> Layoutlist { get; set; }
        public List<DropdownOutputDto> SortList { get; set; }
        public List<DropdownOutputDto> PGPKeyList { get; set; }
        public List<DropdownOutputDto> ShipToList { get; set; }
        public List<DropdownOutputDto> MediaList { get; set; }

        public string LayoutDescription { get; set; }
        public string ShipCCEmail { get; set; }
        public string ShipSubject { get; set; }
        public string ShipNotes { get; set; }
        public string ShippedDate { get; set; }

    }

    public class EditCampaignsOutputDto
    {

        public int CompanyId { get; set; }
        public string Type { get; set; }
        public string LayoutDescription { get; set; }
        public string Layout { get; set; }
        public bool IsHeaderRow { get; set; }
        public bool IsDataFileOnly { get; set; }
        public bool IsUnzipped { get; set; }
        public string Sort { get; set; }
        public string PGPKey { get; set; }
        public string ShipTo { get; set; }
        public string Email { get; set; }
        public string FTPSite { get; set; }
        public string UserName { get; set; }
        public int LayoutId { get; set; }
        public string ShipCCEmail { get; set; }
        public string ShipSubject { get; set; }
        public string ShipNotes { get; set; }
        public string ShippedDate { get; set; }

        public string Media { get; set; }
        public string FileLabel { get; set; }
        public string FileNotes { get; set; }
        public int? SplitType { get; set; }
        public int? SplitIntoNParts { get; set; }

        public List<EditCampaignExportPartDto> EditCampaignExportPart { get; set; }



    }
}