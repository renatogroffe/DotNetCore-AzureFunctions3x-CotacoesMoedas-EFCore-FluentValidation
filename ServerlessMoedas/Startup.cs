using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ServerlessMoedas.Data;

[assembly: FunctionsStartup(typeof(ServerlessMoedas.Startup))]
namespace ServerlessMoedas
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddEntityFrameworkSqlServer()
                .AddDbContext<MoedasContext>(
                    options => options.UseSqlServer(
                        Environment.GetEnvironmentVariable("BaseCotacoes")));
        }
    }
}