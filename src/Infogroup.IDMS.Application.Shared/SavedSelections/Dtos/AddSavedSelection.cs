using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Infogroup.IDMS.SavedSelections.Dtos
{
    public class AddSavedSelection 
    {
		public int SegmentID { get; set; }

        public int CampaignID { get; set; }

        public List<GetSavedSelectionForViewDto> SavedSelectionList { get; set; }
    }
}