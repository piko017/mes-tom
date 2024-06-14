﻿using TMomAot.Endpoints;

namespace TMomAot.Setup
{
    public static class JsonExtension
    {
        public static IServiceCollection WithJsonConfiguration(this IServiceCollection services)
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                var resolverChain = options.SerializerOptions.TypeInfoResolverChain;
                resolverChain.Insert(0, PrintJsonSerializerContext.Default);
            });

            return services;
        }
    }
}