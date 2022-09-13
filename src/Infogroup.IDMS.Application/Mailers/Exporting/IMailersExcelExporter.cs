using System.Collections.Generic;
using Infogroup.IDMS.Mailers.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Mailers.Exporting
{
    public interface IMailersExcelExporter
    {
        FileDto ExportToFile(List<MailerOfferDto> mailerOffers, string databaseName, string fileName);
    }
}