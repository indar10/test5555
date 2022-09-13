
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Models.Dtos
{
    public class ModelScoringDto : EntityDto
    {
        public int ModelId { get; set; }

        public string cModelNumber { get; set; }

        public string cModelName { get; set; }

        public string cDescription { get; set; }

        public string cDatabaseName { get; set; }

        public string cBuildDescription { get; set; }

        public string cLookupDescription { get; set; }

        public int cStatus { get; set; }

        public string dStatusDate { get; set; }

        public bool iIsActive { get; set; }

        public string ModelType { get; set; }

        public string GiftWeight { get; set; }
    }
}