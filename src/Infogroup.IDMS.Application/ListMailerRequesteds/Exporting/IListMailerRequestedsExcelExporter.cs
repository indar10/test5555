using System.Collections.Generic;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.ListMailerRequesteds.Exporting
{
    public interface IListMailerRequestedsExcelExporter
    {
        FileDto ExportToFile(List<GetListMailerRequestedForViewDto> listMailerRequesteds);
    }
}