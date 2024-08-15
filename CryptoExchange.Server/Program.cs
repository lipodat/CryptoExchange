using CryptoExchange.Base;
using CryptoExchange.Base.Interfaces;
using CryptoExchange.Server.Hubs;
using CryptoExchange.Server.Services;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchange.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.AddHttpClient<BitstampService>();
            builder.Services.AddSingleton<IBitstampService, BitstampService>();
            builder.Services.AddHostedService<BitstampSignalRService>();
            builder.Services.AddDbContextFactory<CryptoExchangeDbContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")),
                            lifetime: ServiceLifetime.Singleton);
            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            using IServiceScope serviceScope = app.Services.CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<CryptoExchangeDbContext>().Database.Migrate();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<OrderBookHub>(Constants.SignalR_OrderBookEndpoint);
            app.Run();
        }
    }
}
