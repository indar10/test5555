using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ListMailers.Dtos
{
    public class GetListMailerForEditOutput
    {
		public CreateOrEditListMailerDto ListMailer { get; set; }

		public string MasterLoLcListName { get; set;}


    }
}