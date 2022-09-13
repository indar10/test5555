using System;
using System.IO;
using Abp;
using Abp.AspNetZeroCore;
using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Configuration;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.MultiTenancy;
using Infogroup.IDMS.Security.Recaptcha;
using Infogroup.IDMS.Test.Base.DependencyInjection;
using Infogroup.IDMS.Test.Base.UiCustomization;
using Infogroup.IDMS.Test.Base.Url;
using Infogroup.IDMS.Test.Base.Web;
using Infogroup.IDMS.UiCustomization;
using Infogroup.IDMS.Url;
using NSubstitute;
using Infogroup.IDMS.Caching;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.States;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Test.Base.Cache;
using Microsoft.AspNetCore.Hosting;
using Infogroup.IDMS.Test.Base.Hosting;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Test.Base.Session;

namespace Infogroup.IDMS.Test.Base
{
    [DependsOn(
        typeof(IDMSApplicationModule),
        typeof(IDMSEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule))]
    public class IDMSTestBaseModule : AbpModule
    {
        public IDMSTestBaseModule(IDMSEntityFrameworkCoreModule abpZeroTemplateEntityFrameworkCoreModule)
        {
            abpZeroTemplateEntityFrameworkCoreModule.SkipDbContextRegistration = true;
        }

        public override void PreInitialize()
        {
            var configuration = GetConfiguration();

            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
            Configuration.UnitOfWork.IsTransactional = false;

            //Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            RegisterFakeService<AbpZeroDbMigrator>();

            IocManager.Register<IAppUrlService, FakeAppUrlService>();
            IocManager.Register<IWebUrlService, FakeWebUrlService>();
            IocManager.Register<IRecaptchaValidator, FakeRecaptchaValidator>();

            IocManager.Register<IRedisCacheHelper, FakeRedisCacheHelper>();
            IocManager.Register<IRedisConfiguration, FakeRedisConfiguration>();
            IocManager.Register<IHostingEnvironment, FakeHostingEnvironment>();
            IocManager.Register<IAppSession, FakeAppSession>();


            IocManager.Register<IRedisCacheDatabaseProvider, RedisCacheDatabaseProvider>();
            IocManager.Register<IRedisIDMSConfigurationCache, RedisIDMSConfigurationCache>();
            IocManager.Register<IRedisStateCache, RedisStateCache>();
            IocManager.Register<IRedisLookupCache, RedisLookupCache>();
            IocManager.Register<IRedisIDMSUserCache, RedisIDMSUserCache>();
            IocManager.Register<IRedisDatabaseCache, RedisDatabaseCache>();
            IocManager.Register<IRedisBuildCache, RedisBuildCache>();
            IocManager.Register<IShortSearch, ShortSearch.ShortSearch>();



            Configuration.ReplaceService<IAppConfigurationAccessor, TestAppConfigurationAccessor>();
            Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IUiThemeCustomizerFactory, NullUiThemeCustomizerFactory>();

            Configuration.Modules.AspNetZero().LicenseCode = configuration["AbpZeroLicenseCode"];

            //Uncomment below line to write change logs for the entities below:
            Configuration.EntityHistory.IsEnabled = true;
            Configuration.EntityHistory.Selectors.Add("IDMSEntities", typeof(User), typeof(Tenant));
        }

        public override void Initialize()
        {
            ServiceCollectionRegistrar.Register(IocManager);
        }

        private void RegisterFakeService<TService>()
            where TService : class
        {
            IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton()
            );
        }

        private static IConfigurationRoot GetConfiguration()
        {
            return AppConfigurations.Get(Directory.GetCurrentDirectory(), addUserSecrets: true);
        }
    }
}
