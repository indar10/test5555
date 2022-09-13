using System.Collections.Generic;
using Infogroup.IDMS.Chat.Dto;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(List<ChatMessageExportDto> messages);
    }
}
