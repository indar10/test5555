
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.SecurityGroups.Dtos
{
    public class CreateOrEditSecurityGroupDto : EntityDto
    {
        public int DatabaseId { get; set; }

        public string cGroupName { get; set; }

        public string cGroupDescription { get; set; }

        public string cStatus { get; set; }

        public bool iIsActive { get; set; }

        public string cCreatedBy { get; set; }

        public string cModifiedBy { get; set; }

        public DateTime dCreatedDate { get; set; }

        public DateTime? dModifiedDate { get; set; }
    }
}