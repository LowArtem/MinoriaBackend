using MinoriaBackend.Api.Configurations.Hangfire;
using MinoriaBackend.Api.Extensions.Application;
using AutoMapper;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using MinoriaBackend.Api.Services.ImageStoringService;
using MinoriaBackend.Core.Services.Transaction;
using MinoriaBackend.Data.Services.Auth;
using MinoriaBackend.Data.Services.TransactionHistory;
using MinoriaBackend.Data.Services.TransactionService;
using MinoriaBackend.Data.Services.TransactionService.Strategies;
using Prometheus;
using Serilog;

namespace MinoriaBackend.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddBaseModuleDi("DefaultConnection", Configuration);

        #region Hangfire

        // Add Hangfire services
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"), new PostgreSqlStorageOptions
            {
                DistributedLockTimeout = TimeSpan.FromMinutes(1),
                SchemaName = "hangfire"
            }));

        // Add the processing server as IHostedService
        services.AddHangfireServer();

        #endregion

        // Services can be added here
        services.AddTransient(typeof(UserService), typeof(UserService));
        services.AddTransient(typeof(IImageStoringService), typeof(MinioService));
        
        services.AddScoped<IncomeSpendingTransactionStrategy>();
        services.AddScoped<TransferTransactionStrategy>();
        services.AddScoped<ReservationTransactionStrategy>();
        services.AddScoped<ITransactionStrategyFactory, TransactionStrategyFactory>();
        
        services.AddTransient(typeof(TransactionService), typeof(TransactionService));
        services.AddTransient(typeof(TransactionHistoryService), typeof(TransactionHistoryService));

        // Fluent Validation configurations
        services.AddValidatorsFromAssemblyContaining<Startup>();

        // Auto Mapper Configurations
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MinoriaBackend.Api.Mappers.MappingProfile());
            mc.AddProfile(new MinoriaBackend.Data.Mappers.MappingProfile());
        });
        var mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app,
        IApiVersionDescriptionProvider provider,
        IWebHostEnvironment env,
        ILogger<Startup> logger)
    {
        app.MigrateDatabase(logger);

        app.UseBaseServices(env, provider);

        app.UseHangfireDashboard("/api/hangfire", new DashboardOptions
        {
            Authorization = new[] { new AllowAllConnectionsFilter() },
            IgnoreAntiforgeryToken = true
        });

        app.UseSerilogRequestLogging();
        
        app.UseHttpMetrics();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
        });
    }
}