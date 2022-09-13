
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.MatchAppendDatabaseUsers.Dtos
{
    public class CreateOrEditMatchAppendDatabaseUserDto : EntityDto<int?>
    {
        public int UserID { get; set; }
        public int DatabaseID { get; set; }
        public string cCreatedDate { get; set; }
        public string cCreatedBy { get; set; }
    }
}