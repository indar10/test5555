using Abp.Domain.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Caching;
using StackExchange.Redis;
using Infogroup.IDMS.Neighborhoods;

namespace Infogroup.IDMS.States
{
    public class RedisStateCache : IRedisStateCache
    {
        private readonly IRedisCacheHelper _redisHelper;
        private readonly IRepository<State, int> _stateRepository;
        private readonly IRepository<Neighborhood, int> _neighborhoodRepository;
        private readonly string keyPrefix = "SCC";

        public RedisStateCache(IRepository<State, int> stateRepository, IRedisCacheHelper redisHelper, IRepository<Neighborhood> neighborhoodRepository)
        {
            _redisHelper = redisHelper;
            _stateRepository = stateRepository;
            _neighborhoodRepository = neighborhoodRepository;
        }

        public int GetDatabase(int databaseId)
        {
            var key = $"{keyPrefix}_DATABASES";
            var databaseIDs = new List<int>();
            try
            {
                var stateDBCache = _redisHelper.GetString(key);
                if (stateDBCache == null)
                {
                    databaseIDs = _stateRepository.GetAll().Select(state => state.DatabaseID).Distinct().ToList();
                    _redisHelper.SetString(key, JsonConvert.SerializeObject(databaseIDs));
                }
                else
                    databaseIDs = JsonConvert.DeserializeObject<List<int>>(stateDBCache);
                return databaseIDs.Contains(databaseId) ? databaseId : 0;
            }
            catch (RedisConnectionException)
            {
                return _stateRepository.GetAll().Any(state => state.DatabaseID == databaseId) ? databaseId : 0;
            }
        }
        public List<DropdownOutputDto> GetState(int databaseId, string databaseType)
        {
            var databaseTypePart = databaseId == 0 ? $"_{databaseType}" : string.Empty;
            var key = $"{keyPrefix}_{databaseId}{databaseTypePart}_STATES";
            try
            {
                var statesCache = _redisHelper.GetString(key);
                if (statesCache == null)
                {
                    var states = FetchStates(databaseId, databaseType);
                    _redisHelper.SetString(key, JsonConvert.SerializeObject(states));
                    return states;
                }
                return JsonConvert.DeserializeObject<List<DropdownOutputDto>>(statesCache);
            }
            catch (RedisConnectionException)
            {
                return FetchStates(databaseId, databaseType);
            }
        }
        public List<DropdownOutputDto> GetCounty(string cStateCode, int databaseId)
        {
            var key = $"{keyPrefix}_{databaseId}_{cStateCode}_COUNTY";
            try
            {
                var countyCache = _redisHelper.GetString(key);
                if (countyCache == null)
                {
                    var counties = FetchCounty(cStateCode, databaseId);
                    if (counties.Count > 0)
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(counties));
                    return counties;
                }
                return JsonConvert.DeserializeObject<List<DropdownOutputDto>>(countyCache);
            }
            catch (RedisConnectionException)
            {
                return FetchCounty(cStateCode, databaseId);
            }
        }
        public List<DropdownOutputDto> GetCity(string cStateCode, string cCountyCode, int databaseId)
        {
            var countyCode = !string.IsNullOrEmpty(cCountyCode) ? $"_{cCountyCode}" : string.Empty;
            var key = $"{keyPrefix}_{databaseId}_{cStateCode}{countyCode}_CITY";
            try
            {
                var cityCache = _redisHelper.GetString(key);
                if (cityCache == null)
                {
                    var cities = FetchCity(cStateCode, cCountyCode, databaseId);
                    if (cities.Count > 0)
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(cities));
                    return cities;
                }
                return JsonConvert.DeserializeObject<List<DropdownOutputDto>>(cityCache);
            }
            catch (RedisConnectionException)
            {
                return FetchCity(cStateCode, cCountyCode, databaseId);
            }
        }

        public List<DropdownOutputDto> GetNeighborhood(string cStateCode,int databaseId,string city)
        {
            var key = $"{keyPrefix}_{databaseId}_{cStateCode}_{city}_NEIGHBORHOOD";
            try
            {
                var neighborhoodCache = _redisHelper.GetString(key);
                if (neighborhoodCache == null)
                {
                    var neighborhoods = FetchNeighborhood(cStateCode,city);
                    if (neighborhoods.Count > 0)
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(neighborhoods));
                    return neighborhoods;
                }
                return JsonConvert.DeserializeObject<List<DropdownOutputDto>>(neighborhoodCache);
            }
            catch (RedisConnectionException)
            {
                return FetchNeighborhood(cStateCode,city);
            }
        }

        private List<DropdownOutputDto> FetchStates(int databaseId, string databaseType)
        {
            var getStatesQuery = _stateRepository.GetAll().Where(state => !string.IsNullOrWhiteSpace(state.cState));
            if (databaseId > 0)
                getStatesQuery = getStatesQuery.Where(state => state.DatabaseID == databaseId);
            else if (databaseType == "U")
                getStatesQuery = getStatesQuery.Where(state => state.cZip != "NA");
            else
                getStatesQuery = getStatesQuery.Where(state => state.cZip == "NA");
            var result = getStatesQuery
                .Select(state => new DropdownOutputDto { Label = state.cState, Value = state.cStateCode })
                .Distinct()
                .OrderBy(state => state.Label)
                .ToList();
            return result;
        }
        private List<DropdownOutputDto> FetchCounty(string cStateCode, int databaseId)
        {
            return (from state in _stateRepository.GetAll()
                    where state.cStateCode == cStateCode
                    && state.DatabaseID == databaseId
                    orderby state.cCountyCode
                    group state by new { state.cCountyCode, state.cCounty } into county
                    select new DropdownOutputDto { Label = county.Key.cCounty, Value = county.Key.cCountyCode }).ToList();
        }
        private List<DropdownOutputDto> FetchCity(string cStateCode, string cCountyCode, int databaseId)
        {
            var query = (from state in _stateRepository.GetAll()
                         where state.cStateCode == cStateCode
                         && state.DatabaseID == databaseId
                         select state);
            if (!string.IsNullOrEmpty(cCountyCode))
                query = query.Where(state => state.cCountyCode == cCountyCode);
            return (from state in query
                    orderby state.cCity
                    group state by state.cCity into city
                    select new DropdownOutputDto { Label = city.Key, Value = city.Key }).ToList();
        }

        private List<DropdownOutputDto> FetchNeighborhood(string cStateCode,string city)
        {
            var query = (from neighborhood in _neighborhoodRepository.GetAll()
                         where neighborhood.cState == cStateCode && neighborhood.cCity==city
                         orderby neighborhood.cNeighborhood
                         group neighborhood by neighborhood.cNeighborhood into NH
                         select new DropdownOutputDto { Label = NH.Key, Value = NH.Key }).ToList();
            return query;

        }
    }
}