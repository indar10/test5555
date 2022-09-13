using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SavedSelectionDetails.Dtos
{
    public class GetAllSavedSelectionDetailsInput 
    {	
		 public int segmentID { get; set; }

		 public int savedSelectionID { get; set; }

         public bool userDefault { get; set; }
    }
}