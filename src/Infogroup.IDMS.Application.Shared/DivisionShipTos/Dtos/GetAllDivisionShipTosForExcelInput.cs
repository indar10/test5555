using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.DivisionShipTos.Dtos
{
    public class GetAllDivisionShipTosForExcelInput
    {
		public string Filter { get; set; }

		public string cCodeFilter { get; set; }

		public string cCompanyFilter { get; set; }

		public string cFirstNameFilter { get; set; }

		public string cLastNameFilter { get; set; }

		public string cEmailFilter { get; set; }

		public int iIsActiveFilter { get; set; }



    }
}