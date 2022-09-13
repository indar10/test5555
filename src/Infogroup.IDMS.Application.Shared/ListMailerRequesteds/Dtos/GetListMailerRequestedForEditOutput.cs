using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ListMailerRequesteds.Dtos
{
    public class GetListMailerRequestedForEditOutput
    {
		public CreateOrEditListMailerRequestedDto ListMailerRequested { get; set; }

		public string MasterLoLcListName { get; set;}


    }
}