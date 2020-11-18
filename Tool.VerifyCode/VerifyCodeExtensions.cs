using System;
using System.Collections.Generic;
using System.Text;
using Tool.VerifyCode;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class VerifyCodeExtensions
    {
        public static void AddVerifyCodeService(this IServiceCollection services, Action<VerifyCodeOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddTransient<SlideVerifyCode>();
            services.AddTransient<VerifyCode>();
            services.AddScoped<IVerifyCodeService, VerifyCodeService>();

            services.Configure(setupAction);
        }
    }
}
