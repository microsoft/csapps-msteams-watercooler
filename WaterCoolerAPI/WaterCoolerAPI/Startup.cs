// <copyright file="Startup.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WaterCoolerAPI
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.AzureAD.UI;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Graph.Communications.Common.Telemetry;
    using Microsoft.IdentityModel.Tokens;
    using WaterCoolerAPI.Common;
    using WaterCoolerAPI.Extensions;
    using WaterCoolerAPI.Helpers;
    using WaterCoolerAPI.Interface;
    using WaterCoolerAPI.Repositories;
    using WaterCoolerAPI.Repositories.ParticipantData;
    using WaterCoolerAPI.Repositories.RoomData;
    using WaterCoolerAPI.Repositories.RoomLogoUrlData;
    using WaterCoolerAPI.Repositories.TeamsLogoUrlData;

    /// <summary>
    /// Register services in DI container, and set up middle-wares in the pipeline.
    /// </summary>
    public class Startup
    {
        private readonly GraphLogger logger;

        private readonly IAzureBlobHelper azureBlobHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration instance.</param>
        public Startup(IConfiguration configuration)
        {
            this.azureBlobHelper = azureBlobHelper;
            this.Configuration = configuration;
            this.logger = new GraphLogger(typeof(Startup).Assembly.GetName().Name);
        }

        /// <summary>
        /// Gets the IConfiguration instance.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">IServiceCollection instance.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddHttpClient(Constants.GraphWebClient, client => client.Timeout = TimeSpan.FromSeconds(600));
            services.AddOptions<RepositoryOptions>()
                .Configure<IConfiguration>((repositoryOptions, configuration) =>
                {
                    repositoryOptions.StorageAccountConnectionString =
                        configuration.GetValue<string>(Constants.StorageAccountConnectionStringConfigurationSettingsKey);

                    repositoryOptions.BlobContainerName =
                        configuration.GetValue<string>(Constants.BlobContainerNameConfigurationSettingsKey);

                    // Setting this to true because the main application should ensure that all
                    // tables exist.
                    repositoryOptions.EnsureTableExists = true;
                });

            services.AddSingleton<IGraphLogger>(this.logger)
                .AddAuthentication(sharedOptions =>
                        {
                            sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                    .AddJwtBearer(options =>
                    {
                        var azureAdOptions = new AzureADOptions();
                        this.Configuration.Bind(Constants.AzureAdConfigurationSettingsKey, azureAdOptions);
                        options.Authority = $"{azureAdOptions.Instance}{azureAdOptions.TenantId}/v2.0";
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidAudiences = AuthenticationHelper.GetValidAudiences(this.Configuration),
                            ValidIssuers = AuthenticationHelper.GetValidIssuers(this.Configuration),
                            AudienceValidator = AuthenticationHelper.AudienceValidator,
                        };
                    });

            // Add repositories.
            services.AddSingleton<IRoomDataRepository, RoomDataRepository>();
            services.AddSingleton<IParticipantDataRepository, ParticipantDataRepository>();

            services.AddTransient<TableRowKeyGenerator>();

            services.AddSingleton<IGraph, GraphHelper>();
            services.AddSingleton<IActiveRoomAndParticipant, ActiveRoomAndParticipantHelper>();
            services.AddSingleton<IRoomLogoDataRepository, RoomLogoDataRepository>();
            services.AddSingleton<IAzureBlobHelper, AzureBlobStorageHelper>();
            services.AddCors(options =>
            {
                options.AddPolicy(
                "AllowAll",
                builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
            });
            services.AddApplicationInsightsTelemetry();

            services.AddBot(options => this.Configuration.Bind(Constants.BotConfigurationSettingsKey, options));

            services.AddCors(options =>
            {
                options.AddPolicy(
                    Constants.CORSAllowAllPolicy,
                    builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
            });

            services.AddHostedService<ParticipantsScheduler>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder instance, which is a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">IHostingEnvironment instance, which provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAzureBlobHelper blobHelper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(Constants.CORSAllowAllPolicy);

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            /// <summary>
            /// Invoke the code for azure blob storage container creation and blob upload on app startup.
            /// </summary>
            bool containerExists = Task.Run(async () => await blobHelper.CheckOrCreateBlobContainer()).Result;
            if (containerExists)
            {
                bool blobsUploaded= Task.Run(async () => await blobHelper.BlobUpload()).Result;
            }
        }
    }
}
