using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Models
{
    public interface IModelsRepository : IRepository<Model, int>
    {
        PagedResultDto<ModelScoringDto> GetAllModelsList(Tuple<string, string, List<SqlParameter>> query);

        CreateOrEditModelDto GetModelById(Tuple<string, string, List<SqlParameter>> query);

        int GetChildTableNumber(Tuple<string, List<SqlParameter>> query);
        
        List<ModelStatusDto> GetModelStatusForModelDetail(int modelDetailId);
    }
}
