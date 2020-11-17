using System;
using System.Collections.Generic;
using System.Text;
using Tool.Sms.Aliyun;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmsExtensions
    {
        public static void AddSmsService(this IServiceCollection services, Action<SmsOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }
            services.AddScoped<ISmsService, SmsService>();
            //setupAction(new StorageOptions());
            services.Configure(setupAction);
        }
    }
}
