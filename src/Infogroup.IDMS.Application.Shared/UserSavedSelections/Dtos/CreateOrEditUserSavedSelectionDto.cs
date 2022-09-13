
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserSavedSelections.Dtos
{
    public class CreateOrEditUserSavedSelectionDto : EntityDto<int?>
    {
        public string cDescription { get; set; }
        public bool iIsActive { get; set; }
        public string cChannelType { get; set; }
        public bool iIsDefault { get; set; }
        public int UserID { get; set; }
        public int DatabaseId { get; set; }
        public string cCreatedBy { get; set; }
        public DateTime dCreatedDate { get; set; }
        public string cModifiedBy { get; set; }
        public DateTime? dModifiedDate { get; set; }
    }

}