using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Mailers.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Mailers
{
    public interface IMailerRepository : IRepository<Mailer, int>
    {
        PagedResultDto<MailerDto> GetAllMailersList(Tuple<string, string, List<SqlParameter>> query);
    }
}
