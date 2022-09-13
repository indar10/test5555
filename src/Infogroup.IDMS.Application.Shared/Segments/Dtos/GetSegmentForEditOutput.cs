using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class GetSegmentForEditOutput
    {
		public CreateOrEditSegmentDto Segment { get; set; }

		public string OrdercDatabaseName { get; set;}


    }
}