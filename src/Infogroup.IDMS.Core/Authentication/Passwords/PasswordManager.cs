using Abp.Domain.Repositories;
using Abp.UI;
using System;
using System.Linq;
using Infogroup.IDMS.AbpUserPasswords;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Lookups;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Infogroup.IDMS.Authorization
{
    public class PasswordManager : IDMSDomainServiceBase, IPasswordManager
    {
        private readonly IRepository<AbpUserPassword, long> _abpUserPasswordRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly List<long> adminUserIds;
        private readonly IRepository<User, long> _abpuser;
        private readonly IRedisLookupCache _lookupCache;
        
        public PasswordManager(IRepository<AbpUserPassword, long> abpUserPasswordRepository, 
            IRepository<User, long> abpuser,
            IPasswordHasher<User> passwordHasher,
            IRedisLookupCache lookupCache)
        {
            _abpUserPasswordRepository = abpUserPasswordRepository;
            _abpuser = abpuser;
            _lookupCache = lookupCache;
            _passwordHasher = passwordHasher;
            adminUserIds = new List<long> { 1, 2 };
        }
        public void CheckRecentPasswords(User user, string newPassword)
        {
            try
            {
                if (!adminUserIds.Contains(user.Id))
                {
                    // Get recent 8 passwords
                    var recentPasswords = _abpUserPasswordRepository.GetAll()
                        .Where(password => password.UserId == user.Id)
                        .OrderByDescending(password => password.Id)
                        .Take(8)
                        .Select(password => password.Password)
                        .ToList();
                    foreach (var recentPassword in recentPasswords)
                    {
                        var verificationResult = _passwordHasher.VerifyHashedPassword(user, recentPassword, newPassword);
                        if (verificationResult == PasswordVerificationResult.Success)
                            throw new UserFriendlyException(L("RecentlyUsedPasswordError"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public string GetLastPasswordChange(string UserNameOrEmailAddress)
        {
            //check if input is username or email
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(UserNameOrEmailAddress);
            DateTime passwordCreationTime;
            if (match.Success)
            {
                passwordCreationTime = (from user in _abpuser.GetAll()
                                            join userpasswords in _abpUserPasswordRepository.GetAll()
                                            on user.Id equals userpasswords.UserId
                                            where user.EmailAddress.ToLower() == UserNameOrEmailAddress.Trim().ToLower()
                                            orderby userpasswords.Id descending
                                            select userpasswords.CreationTime).FirstOrDefault();
            }
            else
            {
                passwordCreationTime = (from user in _abpuser.GetAll()
                                            join userpasswords in _abpUserPasswordRepository.GetAll()
                                            on user.Id equals userpasswords.UserId
                                            where user.UserName.ToLower() == UserNameOrEmailAddress.Trim().ToLower()
                                            orderby userpasswords.Id descending
                                            select userpasswords.CreationTime).FirstOrDefault();
            }

            TimeSpan tmSpan;
            var iSpan = 0;
            var returnMessage = string.Empty;
            if (passwordCreationTime != null && passwordCreationTime != DateTime.MinValue)
            {
                tmSpan = DateTime.Now - Convert.ToDateTime(passwordCreationTime);
                iSpan = Convert.ToInt32(_lookupCache.GetLookUpFields("EXPPASS").FirstOrDefault().cDescription);
                if (tmSpan.Days >= iSpan - 8)
                {
                    if (tmSpan.Days >= iSpan)
                    {
                        // Lock user
                        returnMessage = L("AccountLocked");
                    }
                    else
                        returnMessage = L("PasswordWillExpire", iSpan - tmSpan.Days);
                }               
            }
            return returnMessage;
        }
    }
}