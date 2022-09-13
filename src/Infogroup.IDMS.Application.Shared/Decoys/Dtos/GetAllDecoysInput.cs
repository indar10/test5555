using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Decoys.Dtos
{
    public class GetAllDecoysInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

        public int SelectedDatabase { get; set; }

        public int mailerId { get; set; }

    }
}