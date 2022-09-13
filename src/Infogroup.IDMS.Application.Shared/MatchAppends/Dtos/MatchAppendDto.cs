
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class MatchAppendDto : EntityDto
    {
        public int DatabaseID { get; set; }

        public int BuildID { get; set; }

        
        public string UploadFilePath { get; set; }

        
        public string LK_FileType { get; set; }

        
        public string LK_ExportFileFormatID { get; set; }

        
        public string cOrderType { get; set; }

        
        public string cKeyCode { get; set; }

        public bool iSkipFirstRow { get; set; }

        
        public string cClientName { get; set; }

        
        public string cRequestReason { get; set; }

        
        public string cSourceFilter { get; set; }

        
        public string cInputFilter { get; set; }

        
        public string cIDMSMatchFieldName { get; set; }

       
        public string cInputMatchFieldName { get; set; }

        public DateTime dCreatedDate { get; set; }

        
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

        
        public string cClientFileName { get; set; }

        public int iExportType { get; set; }

        public string cBuildDescription { get; set; }

    }
}