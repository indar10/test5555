using Abp.Application.Services;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Logging.Dto;

namespace Infogroup.IDMS.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
