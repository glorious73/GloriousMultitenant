using Application.Infrastructure.AppData;
using System.Security.Cryptography;
using WebAPI.Configuration.Db;
using WebAPI.Middleware;

namespace WebAPI.Configuration
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseTiming(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TimingMiddleware>();
        }

        public static void SeedDb(this IApplicationBuilder app)
        {
            IServiceScopeFactory scopedFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopedFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<IDbSeedConfig>();
                service.Seed();
            }
        }

        public static void AssignGlobalData(this IApplicationBuilder app, RSA? encryptionKey, ECDsa? signingKey)
        {
            var scopedFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopedFactory.CreateScope())
            {
                var appData = scope.ServiceProvider.GetService<IAppData>();
                appData.EncryptionKey = encryptionKey;
                appData.SigningKey = signingKey;
            }
        }
    }
}
