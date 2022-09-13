using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Decoys.Dtos;
using Infogroup.IDMS.Mailers.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Decoys
{
    public interface IDecoyRepository : IRepository<Decoy, int>
    {
        Task<List<DecoyDto>> GetDecoyEntityListByMailer(int mailerID);
        PagedResultDto<MailerDto> GetAllDecoyMailers(int userId, GetAllDecoysInput input, string shortWhere);
        PagedResultDto<DecoyDto> GetDecoysByMailer(GetAllDecoysInput input, string shortWhere);
        void CopyDecoy(int decoyId, string userName);


    }
}
