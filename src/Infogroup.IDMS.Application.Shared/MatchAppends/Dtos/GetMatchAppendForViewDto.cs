using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class GetMatchAppendForViewDto :EntityDto
    {
        public int DatabaseID { get; set; }

        public string DatabaseName { get; set; }

        public string BuildDescription { get; set; }

        public string Status { get; set; }

        public int BuildID { get; set; }

        public string StatusDate { get; set; }       

        public string cClientName { get; set; }

        public string cRequestReason { get; set; }

        public int StatusId { get; set; }

        public string IDMSMatchFieldName { get; set; }

        public string cClientFileName { get; set; }


    }
}