using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infogroup.IDMS.Occupations.Dtos;
using Abp.Authorization;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Abp.UI;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Shared.Dtos;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Occupations
{
    [AbpAuthorize]
    public class OccupationsAppService : IDMSAppServiceBase, IOccupationsAppService
    {
        private readonly IOccupationsRepository _customOccupationRepository;
        private readonly IBuildTableLayoutManager _buildTableLayoutManager;

        public OccupationsAppService(IOccupationsRepository customOccupationRepository, IBuildTableLayoutManager buildTableLayoutManager)
        {
            _customOccupationRepository = customOccupationRepository;
            _buildTableLayoutManager = buildTableLayoutManager;

        }

        public async Task<GetOccupationForViewDto> GetInitialData(GetAdvanceFieldDetailsInputDto input)
        {
            try
            {
                var result = new GetOccupationForViewDto();
                result.ConfiguredFields = await _buildTableLayoutManager.GetAdvanceSelectionFieldDetails(input);
                var query = new QueryBuilder();
                query.AddDistinct();
                query.AddSelect(@" LTRIM(RTRIM(occupation.cIndustryCode)) as Code
                                  ,LTRIM(RTRIM(occupation.cIndustry)) as Description");
                query.AddFrom("tblOccupation", "occupation");
                query.AddNoLock();
                (string sqlQuery, List<SqlParameter> sqlParams) = query.Build();
                result.Industries = _customOccupationRepository.GetOccupationValues(sqlQuery, sqlParams);
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }

        public List<DropdownOutputDto> GetAllOccupationByIndustry(string industryCode)
        {
            try
            {
                var query = new QueryBuilder();
                query.AddDistinct();
                query.AddSelect(@" LTRIM(RTRIM(occupation.cOccupationCode)) as Code
                                  ,LTRIM(RTRIM(occupation.cOccupationtitle)) as Description");
                query.AddFrom("tblOccupation", "occupation");
                query.AddWhere("AND", "occupation.cIndustryCode", "EQUALTO", industryCode);
                query.AddWhereString("AND  occupation.cOccupationCode <> '' ");
                query.AddWhereString("AND  occupation.cOccupationtitle <> '' ");
                query.AddSort("Description ASC");
                query.AddNoLock();
                (string sqlQuery, List<SqlParameter> sqlParams) = query.Build();
                return _customOccupationRepository.GetOccupationValues(sqlQuery, sqlParams);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }

        public List<DropdownOutputDto> GetAllSpecialtyTitleByIndustryOccupation(string industryCode, string occupationCode)
        {
            try
            {
                var query = new QueryBuilder();
                query.AddDistinct();
                query.AddSelect(@" LTRIM(RTRIM(occupation.cSpecialityCode)) as Code
                                  ,LTRIM(RTRIM(occupation.cSpecialtytitle)) as Description");
                query.AddFrom("tblOccupation", "occupation");
                query.AddWhere("AND", "occupation.cIndustryCode", "EQUALTO", industryCode);
                query.AddWhere("AND", "occupation.cOccupationCode", "EQUALTO", occupationCode);
                query.AddWhereString("AND  occupation.cSpecialityCode <> '' ");
                query.AddWhereString("AND  occupation.cSpecialtytitle <> '' ");
                query.AddSort("Description ASC");
                query.AddNoLock();
                (string sqlQuery, List<SqlParameter> sqlParams) = query.Build();
                return _customOccupationRepository.GetOccupationValues(sqlQuery, sqlParams);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
    }
}