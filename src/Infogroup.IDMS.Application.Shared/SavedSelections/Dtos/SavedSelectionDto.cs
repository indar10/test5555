
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SavedSelections.Dtos
{
    public class SavedSelectionDto : EntityDto
    {

        public int GroupID { get; set; }

        public string cDescription { get; set; }

        public bool iIsActive { get; set; }

        public DateTime dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

        public int DatabaseId { get; set; }

    }
}