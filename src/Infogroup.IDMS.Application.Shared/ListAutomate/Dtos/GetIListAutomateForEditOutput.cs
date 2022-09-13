using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ListAutomate.Dtos
{
    public class GetIListAutomateForEditOutput
    {
        public CreateOrEditIListAutomateDto IListAutomate { get; set; }

    }
}