﻿using Feijuca.MultiTenancy.Services.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.CrossCutting.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, AuthSettings authSettings)
        {
            services.AddHttpClient("KeycloakClient", client =>
            {
                client.BaseAddress = new Uri(authSettings.AuthServerUrl!);
            });

            return services;
        }
    }
}
