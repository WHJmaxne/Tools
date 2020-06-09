using System;
using System.Collections.Generic;
using System.Text;
using Tool.Azure.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureStorageExtensions
    {
        public static void AddAzureStorage(this IServiceCollection services, Action<StorageOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }
            services.AddScoped<IAzureStorageService, AzureStorageService>();
            //setupAction(new StorageOptions());
            services.Configure(setupAction);
        }
    }
}
