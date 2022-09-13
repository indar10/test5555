using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.IDMSConfigurations.Dtos
{
    public class IDMSConfigurationDto : EntityDto
    {
        public string cValue { get; set; }
        public int iValue { get; set; }
        public string mValue { get; set; }
        public bool iIsEncrypted { get; set; }
    }
}
