﻿using GraphQL;
using GraphQL.Server;
using Microsoft.Extensions.DependencyInjection;
using Infogroup.IDMS.Debugging;

namespace Infogroup.IDMS.Configure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAndConfigureGraphQL(this IServiceCollection services)
        {
            services.AddScoped<IDependencyResolver>(
                x => new FuncDependencyResolver(x.GetRequiredService)
            );

            services
                .AddGraphQL(x => { x.ExposeExceptions = DebugHelper.IsDebug; })
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddUserContextBuilder(httpContext => httpContext.User)
                .AddDataLoader();
        }
    }
}