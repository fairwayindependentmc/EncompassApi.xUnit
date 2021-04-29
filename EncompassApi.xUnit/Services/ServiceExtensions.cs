using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EncompassApi.xUnit.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMockedEncompassHttpClient(this IServiceCollection services)
        {
            services.AddTransient<IMockedEncompassHttpClientService, MockedEncompassHttpClientService>();
            return services;
        }
    }
}
