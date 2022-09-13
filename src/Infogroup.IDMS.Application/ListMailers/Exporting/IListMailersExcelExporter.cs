using System.Collections.Generic;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.ListMailers.Exporting
{
    public interface IListMailersExcelExporter
    {
        FileDto ExportToFile(List<GetListMailerForViewDto> listMailers);
    }
}