using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class GetAllIDMSTasksInput
    {
		public string Filter { get; set; }

        public string Sorting { get; set; }

    }
}