
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserDivisions.Dtos
{
    public class UserDivisionDto : EntityDto
    {

        public int UserID { get; set; }

        public int DivisionID { get; set; }
        public int iSelectedBuildID { get; set; }

        public int iSelectedDatabaseID { get; set; }

        public string cCreatedBy { get; set; }

        public DateTime dCreatedDate { get; set; }

        public string cModifiedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }


    }
}