using Abp.Domain.Repositories;
using Abp.UI;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.Caching;
using Infogroup.IDMS.CampaignFavourites;
using Infogroup.IDMS.CampaignFavourites.Dtos;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Divisions;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.UserAccessObjects;
using Infogroup.IDMS.UserDatabaseAccessObjects;
using Infogroup.IDMS.UserDatabaseMailers;
using Infogroup.IDMS.UserDatabaseMailers.Dtos;
using Infogroup.IDMS.UserDatabases;
using Infogroup.IDMS.UserDivisions;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infogroup.IDMS.IDMSUsers
{
    public class RedisIDMSUserCache : IRedisIDMSUserCache
    {
        private readonly IRepository<UserDatabase, int> _userDatabaseRepository;
        private readonly IRepository<Database, int> _databaseRepository;
        private readonly IRepository<UserDivision, int> _userDivisionRepository;
        private readonly IRepository<Division, int> _divisionRepository;
        private readonly IRepository<CampaignFavourite> _campaignFavouriteRepository;
        private readonly IRepository<Campaign, int> _campaignRepository;
        private readonly IRepository<Build, int> _buildRepository;
        private readonly IRepository<UserDatabaseMailer, int> _userDatabaseMailerRepository;
        private readonly IRepository<UserDatabaseAccessObject, int> _userDatabaseAccessRepository;
        private readonly IRepository<UserAccessObject, int> _userAccessRepository;
        private readonly IDatabaseRepository _customDatabaseRepository;
        private readonly IRedisCacheHelper _redisHelper;
        private readonly string keyPrefix = "USR";


        public RedisIDMSUserCache(
            IDatabaseRepository customDatabaseRepository,
            IRepository<UserDatabase, int> userDatabaseRepository,
            IRepository<Database, int> databaseRepository,
            IRepository<UserDivision, int> userDivisionRepository,
            IRepository<Division, int> divisionRepository,
            IRepository<CampaignFavourite> campaignFavouriteRepository,
            IRepository<Build, int> buildRepository,
            IRepository<Campaign, int> campaignRepository,
            IRepository<UserDatabaseMailer, int> userDatabaseMailerRepository,
            IRepository<UserAccessObject, int> userAccessRepository,
            IRepository<UserDatabaseAccessObject, int> userDatabaseAccessRepository,
            IRedisCacheHelper redisHelper)
        {
            _customDatabaseRepository = customDatabaseRepository;
            _userDatabaseRepository = userDatabaseRepository;
            _databaseRepository = databaseRepository;
            _userDivisionRepository = userDivisionRepository;
            _divisionRepository = divisionRepository;
            _redisHelper = redisHelper;
            _campaignFavouriteRepository = campaignFavouriteRepository;
            _buildRepository = buildRepository;
            _campaignRepository = campaignRepository;
            _userDatabaseMailerRepository = userDatabaseMailerRepository;
            _userAccessRepository = userAccessRepository;
            _userDatabaseAccessRepository = userDatabaseAccessRepository;
        }

        public void SetDefaultDatabaseForCampaign(int userId, int dataBaseId)
        {
            var key = $"{AppConsts.DefaultDatabaseId}_{userId}";
            var deafultDatabaseCache = _redisHelper.GetString(key);
            _redisHelper.SetString(key, JsonConvert.SerializeObject(dataBaseId));
        }
        public int GetDefaultDatabaseForCampaign(int userId, int abpUserId)
        {
            var defaultDatabaseId = 0;
            try
            {
                var key = $"{AppConsts.DefaultDatabaseId}_{abpUserId}";
                var deafultDatabaseCache = _redisHelper.GetString(key);
                if (deafultDatabaseCache != null)
                    defaultDatabaseId= JsonConvert.DeserializeObject<int>(deafultDatabaseCache);
                else
                    defaultDatabaseId= _userDivisionRepository.GetAll().Where(p => p.UserID == userId && p.iSelectedDatabaseID != 0).OrderByDescending(a => a.Id).FirstOrDefault()?.iSelectedDatabaseID ?? 0;
            }
            catch (RedisConnectionException) {
                defaultDatabaseId= _userDivisionRepository.GetAll().Where(p => p.UserID == userId && p.iSelectedDatabaseID != 0).OrderByDescending(a => a.Id).FirstOrDefault()?.iSelectedDatabaseID ?? 0;
            }
            return defaultDatabaseId;
        }
        public List<DropdownOutputDto> GetDropdownOptions(int iUserID, UserDropdown dropdownType)
        {
            List<DropdownOutputDto> dropdownOptions;
            try
            {
                var key = $"{keyPrefix}_{iUserID}_{dropdownType.ToString()}";
                var userDatabasesCache = _redisHelper.GetString(key);
                if (userDatabasesCache == null)
                {
                    dropdownOptions = FetchDropdownOptions(iUserID, dropdownType);
                    if (dropdownOptions.Count > 0)
                    {
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(dropdownOptions));
                    }
                }
                else
                    dropdownOptions = JsonConvert.DeserializeObject<List<DropdownOutputDto>>(userDatabasesCache);
            }
            catch (RedisConnectionException)
            {
                dropdownOptions = FetchDropdownOptions(iUserID, dropdownType);
            }
            return dropdownOptions;
        }
        public List<CampaignFavouriteDtoForView> GetCampaignFavourites(int iUserID)
        {
            List<CampaignFavouriteDtoForView> dropdownOptions;
            try
            {
                var key = $"{keyPrefix}_{iUserID}_{IDMSUserCacheConsts.FavoriteCampaigns}";
                var userFavCampaignCache = _redisHelper.GetString(key);
                if (userFavCampaignCache == null)
                {
                    dropdownOptions = FetchFavouriteCampaigns(iUserID);
                    if (dropdownOptions.Count > 0)
                    {
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(dropdownOptions));
                    }
                }
                else
                    dropdownOptions = JsonConvert.DeserializeObject<List<CampaignFavouriteDtoForView>>(userFavCampaignCache);
            }
            catch (RedisConnectionException)
            {
                dropdownOptions = FetchFavouriteCampaigns(iUserID);
            }
            return dropdownOptions;
        }

        public List<UserDatabaseAccessObjectCacheItem> GetDatabaseAccessObjects(int iUserID)
        {
            List<UserDatabaseAccessObjectCacheItem> cacheItem;
            try
            {
                var key = $"{keyPrefix}_{iUserID}_DB_ACCESS";
                var dbAccessObjectCache = _redisHelper.GetString(key);
                if (dbAccessObjectCache == null)
                {
                    cacheItem = FetchDatabaseAccessObjects(iUserID);
                    if (cacheItem.Count > 0)
                    {
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(cacheItem));
                    }
                }
                else
                    cacheItem = JsonConvert.DeserializeObject<List<UserDatabaseAccessObjectCacheItem>>(dbAccessObjectCache);
            }
            catch (RedisConnectionException)
            {
                cacheItem = FetchDatabaseAccessObjects(iUserID);
            }
            return cacheItem;
        }

        public List<UserAccessObjectCacheItem> GetAccessObjects(int iUserID)
        {
            List<UserAccessObjectCacheItem> cacheItem;
            try
            {
                var key = $"{keyPrefix}_{iUserID}_ACCESS";
                var accessObjectCache = _redisHelper.GetString(key);
                if (accessObjectCache == null)
                {
                    cacheItem = FetchAccessObjects(iUserID);
                    if (cacheItem.Count > 0)
                    {
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(cacheItem));
                    }
                }
                else
                    cacheItem = JsonConvert.DeserializeObject<List<UserAccessObjectCacheItem>>(accessObjectCache);
            }
            catch (RedisConnectionException)
            {
                cacheItem = FetchAccessObjects(iUserID);
            }
            return cacheItem;
        }

        public List<int> GetDatabaseIDs(int iUserID)
        {
            List<int> userDatabaseIDs;
            try
            {
                var key = $"{keyPrefix}_{iUserID}_DATABASEIDS";
                var userDatabaseIDsCache = _redisHelper.GetString(key);
                if (userDatabaseIDsCache == null)
                {
                    userDatabaseIDs = FetchDatabaseIDs(iUserID);
                    if (userDatabaseIDs.Count > 0)
                    {
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(userDatabaseIDs));
                    }
                }
                else
                    userDatabaseIDs = JsonConvert.DeserializeObject<List<int>>(userDatabaseIDsCache);
            }
            catch (RedisConnectionException)
            {
                userDatabaseIDs = FetchDatabaseIDs(iUserID);
            }
            return userDatabaseIDs;
        }

        public void SetDropdownOptions(int iUserID, UserDropdown dropdownType)
        {
            try
            {
                List<DropdownOutputDto> dropdownOptions;
                var key = $"{keyPrefix}_{iUserID}_{dropdownType.ToString()}";
                dropdownOptions = FetchDropdownOptions(iUserID, dropdownType);
                if (dropdownOptions.Count > 0)
                {
                    _redisHelper.SetString(key, JsonConvert.SerializeObject(dropdownOptions));
                }
            }
            catch (Exception) { }
        }

        public void SetCampaignFavourites(int iUserID)
        {
            try
            {
                List<CampaignFavouriteDtoForView> dropdownOptions;
                var key = $"{keyPrefix}_{iUserID}_{IDMSUserCacheConsts.FavoriteCampaigns}";
                dropdownOptions = FetchFavouriteCampaigns(iUserID);
                if (dropdownOptions.Count > 0)
                {
                    _redisHelper.SetString(key, JsonConvert.SerializeObject(dropdownOptions));
                }
                else
                {
                    _redisHelper.KeyDeleteWithPrefix(key);
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetDatabaseAccessObjects(int iUserID)
        {
            try
            {
                List<UserDatabaseAccessObjectCacheItem> cacheItem;
                var key = $"{keyPrefix}_{iUserID}_DB_ACCESS";
                cacheItem = FetchDatabaseAccessObjects(iUserID);
                if (cacheItem.Count > 0)
                {
                    _redisHelper.SetString(key, JsonConvert.SerializeObject(cacheItem));
                }
                else
                {
                    _redisHelper.KeyDeleteWithPrefix(key);
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetAccessObjects(int iUserID)
        {
            try
            {
                List<UserAccessObjectCacheItem> cacheItem;
                var key = $"{keyPrefix}_{iUserID}_ACCESS";
                cacheItem = FetchAccessObjects(iUserID);
                if (cacheItem.Count > 0)
                    _redisHelper.SetString(key, JsonConvert.SerializeObject(cacheItem));
                else
                    _redisHelper.KeyDeleteWithPrefix(key);
            }
            catch (Exception)
            {
            }
        }

        public void SetDatabaseIDs(int iUserID)
        {
            try
            {
                List<int> userDatabaseIDs;
                var key = $"{keyPrefix}_{iUserID}_DATABASEIDS";
                userDatabaseIDs = FetchDatabaseIDs(iUserID);
                if (userDatabaseIDs.Count > 0)
                {
                    _redisHelper.SetString(key, JsonConvert.SerializeObject(userDatabaseIDs));
                }
            }
            catch (Exception) { }
        }


        private List<DropdownOutputDto> FetchDropdownOptions(int iUserID, UserDropdown dropdownType)
        {
            switch (dropdownType)
            {
                case UserDropdown.Databases:
                    return FetchDatabases(iUserID);
                case UserDropdown.Divisions:
                    return FetchDivisions(iUserID);
                case UserDropdown.DatabaseAccess:
                    return _customDatabaseRepository.FetchDatabasesWithAccess(iUserID);
                default: throw new UserFriendlyException("Invalid Dropdown Type");
            }
        }

        private List<DropdownOutputDto> FetchDatabases(int iUserID)
        {
            return (from userDatabase in _userDatabaseRepository.GetAll()
                    join database in _databaseRepository.GetAll()
                    on userDatabase.DatabaseId equals database.Id
                    where userDatabase.UserId == iUserID
                    orderby database.cDatabaseName
                    select new DropdownOutputDto { Label = $"{database.cDatabaseName} : {database.Id}", Value = database.Id }).ToList();
        }              

        private List<int> FetchDatabaseIDs(int iUserID)
        {
            return (from userDatabase in _userDatabaseRepository.GetAll()
                    join database in _databaseRepository.GetAll()
                    on userDatabase.DatabaseId equals database.Id
                    where userDatabase.UserId == iUserID
                    orderby database.cDatabaseName
                    select database.Id).ToList();
        }

        private List<DropdownOutputDto> FetchDivisions(int iUserID)
        {
            return (from userDivision in _userDivisionRepository.GetAll()
                    join division in _divisionRepository.GetAll()
                    on userDivision.DivisionID equals division.Id
                    where userDivision.UserID == iUserID
                    select new DropdownOutputDto
                    {
                        Value = division.Id,
                        Label = division.cDivisionName
                    }).ToList();
        }

        private List<CampaignFavouriteDtoForView> FetchFavouriteCampaigns(int iUserID)
        {
            try
            {
                return (from campaignFavourite in _campaignFavouriteRepository.GetAll()
                        join campaign in _campaignRepository.GetAll()
                        on campaignFavourite.OrderID equals campaign.Id
                        join build in _buildRepository.GetAll()
                        on campaign.BuildID equals build.Id
                        where campaignFavourite.UserID == iUserID
                        orderby build.Database.cDatabaseName, campaign.cDescription
                        select new CampaignFavouriteDtoForView
                        {
                            CampaignId = campaignFavourite.OrderID,
                            CampaignDescription = campaign.cDescription,
                            DatabaseName = build.Database.cDatabaseName
                        }).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        private List<UserDatabaseAccessObjectCacheItem> FetchDatabaseAccessObjects(int iUserID)
        {
            try
            {
                return _userDatabaseAccessRepository.GetAll()
                    .Where(accessObject => accessObject.UserID == iUserID)
                    .Select(accessObject => new UserDatabaseAccessObjectCacheItem
                    {
                        AccessObjectId = accessObject.AccessObjectId,
                        ListAccess = accessObject.iListAccess,
                        AddEditAccess = accessObject.iAddEditAccess,
                        DatabaseId = accessObject.DatabaseId
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        private List<UserAccessObjectCacheItem> FetchAccessObjects(int iUserID)
        {
            try
            {
                return _userAccessRepository.GetAll()
                    .Where(accessObject => accessObject.UserID == iUserID)
                    .Select(accessObject => new UserAccessObjectCacheItem
                    {
                        AccessObjectId = accessObject.AccessObjectId,
                        ListAccess = accessObject.iListAccess,
                        AddEditAccess = accessObject.iAddEditAccess
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public List<GetUserDatabaseMailerForViewDto> GetUserDatabaseMailerList(int iUserID)
        {
            List<GetUserDatabaseMailerForViewDto> userDatabaseMailerRecords;
            try
            {
                var key = $"{keyPrefix}_{iUserID}_DATABASE_MAILERS";
                var userDatabaseIDsCache = _redisHelper.GetString(key);
                if (userDatabaseIDsCache == null)
                {
                    userDatabaseMailerRecords = FetchUserDatabaseMailerRcordsBasedOnUserId(iUserID);
                    if (userDatabaseMailerRecords.Count > 0)
                    {
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(userDatabaseMailerRecords));
                    }
                }
                else
                    userDatabaseMailerRecords = JsonConvert.DeserializeObject<List<GetUserDatabaseMailerForViewDto>>(userDatabaseIDsCache);
            }
            catch (RedisConnectionException)
            {
                userDatabaseMailerRecords = FetchUserDatabaseMailerRcordsBasedOnUserId(iUserID);
            }
            return userDatabaseMailerRecords;
        }

        private List<GetUserDatabaseMailerForViewDto> FetchUserDatabaseMailerRcordsBasedOnUserId(int userId)
        {
            return (from userDatabaseMailer in _userDatabaseMailerRepository.GetAll()
                    
                    where userDatabaseMailer.UserId == userId
                    
                    select new GetUserDatabaseMailerForViewDto
                    {
                       MailerID=userDatabaseMailer.MailerID,
                       UserId=userDatabaseMailer.UserId,
                       DatabaseId=userDatabaseMailer.DatabaseId
                       
                    }).ToList();
        }
    }
}

