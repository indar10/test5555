using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.States.Dtos;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.Databases;
using Abp.UI;
using System;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Campaigns;
using System.Collections.Generic;
using Abp.Authorization;
using Infogroup.IDMS.BuildTableLayouts.Dtos;

namespace Infogroup.IDMS.States
{
    [AbpAuthorize]
    public class StateAppService : IDMSAppServiceBase, IStateAppService
    {
        private readonly IRedisStateCache _stateCache;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IBuildTableLayoutRepository _buildTableLayoutRepository;
        private readonly IRepository<Campaign, int> _campaignRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IBuildTableLayoutManager _buildTableLayoutManager;
        private readonly IRedisDatabaseCache _databaseCache;


        public StateAppService(
            IRedisStateCache stateCache,
            IRedisIDMSConfigurationCache idmsConfigurationCache,
            IDatabaseRepository databaseRepository,
            IBuildTableLayoutRepository buildTableLayoutRepository,
            IBuildTableLayoutManager buildTableLayoutManager,
            IRedisDatabaseCache databaseCache,
            IRepository<Campaign, int> campaignRepository)
        {
            _stateCache = stateCache;
            _databaseRepository = databaseRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _buildTableLayoutRepository = buildTableLayoutRepository;
            _campaignRepository = campaignRepository;
            _buildTableLayoutManager = buildTableLayoutManager;
            _databaseCache = databaseCache;
        }

        public async Task<GetStateForViewDto> GetState(GetAdvanceFieldDetailsInputDto input)
        {
            try
            {
                var result = new GetStateForViewDto();
                result.ConfiguredFields = await _buildTableLayoutManager.GetAdvanceSelectionFieldDetails(input);
                result.TargetDatabaseId = _stateCache.GetDatabase(input.DatabaseId);
                result.States = new List<DropdownOutputDto>
                {
                    new DropdownOutputDto{ Label="Select State" , Value = string.Empty}
                };
                var newStates = _stateCache.GetState(result.TargetDatabaseId, _databaseCache.GetDatabaseType(input.DatabaseId));
                result.States.AddRange(newStates);
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        public List<DropdownOutputDto> GetCounty(string cStateCode, int databaseID)
        {
            try
            {
                return _stateCache.GetCounty(cStateCode, databaseID);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        public List<DropdownOutputDto> GetCity(string cStateCode, string cCountyCode, int databaseID)
        {
            try
            {
                return _stateCache.GetCity(cStateCode, cCountyCode, databaseID);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        public List<DropdownOutputDto> GetNeighborhood(string cStateCode, int databaseID, string city)
        {

            try
            {
                return _stateCache.GetNeighborhood(cStateCode, databaseID, city);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
    }
}