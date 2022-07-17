using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using TourismSmartTransportation.API.Utilities.Swagger;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Implements;
using TourismSmartTransportation.Business.Implements.Admin;
using TourismSmartTransportation.Business.Implements.Company;
using TourismSmartTransportation.Business.Implements.Mobile.Customer;
using TourismSmartTransportation.Business.Implements.Partner;
using TourismSmartTransportation.Business.Implements.Vehicle;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Company;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.ViewModel.Shared;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.MongoDBContext;
using TourismSmartTransportation.Data.Repositories;
using Vonage.Request;

namespace TourismSmartTransportation.API
{
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
            services.AddDbContext<tourismsmarttransportationContext>(
               options => options.UseSqlServer(Configuration.GetConnectionString("TourismSmartTransportation")));


            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.RequireHttpsMetadata = false;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });

            // Initialize Mongo DB connection
            services.Configure<MongoCosmosDBSettings>(
                Configuration.GetSection(nameof(MongoCosmosDBSettings)));

            services.AddSingleton<IMongoCosmosDBSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoCosmosDBSettings>>().Value);

            services.AddSingleton<MongoDBContext>();
            //------------------

            services.AddScoped<NotAllowedNullPropertiesAttribute>();

            services.AddControllers();

            services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("admin", new OpenApiInfo { Title = "TourismSmartTransportation.API Admin", Version = "admin" });
                c.SwaggerDoc("partner", new OpenApiInfo { Title = "TourismSmartTransportation.API Partner", Version = "partner" });
                c.SwaggerDoc("driver", new OpenApiInfo { Title = "TourismSmartTransportation.API Driver", Version = "driver" });
                c.SwaggerDoc("customer", new OpenApiInfo { Title = "TourismSmartTransportation.API Customer", Version = "customer" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                c.DocumentFilter<CustomSwaggerFilter>();

                c.TagActionsBy(api =>
                {
                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    string controllerName = controllerActionDescriptor.ControllerName;

                    if (api.GroupName != null)
                    {
                        var name = api.GroupName + controllerName.Replace("Controller", "");
                        name = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
                        return new[] { name };
                    }

                    if (controllerActionDescriptor != null)
                    {
                        controllerName = Regex.Replace(controllerName, "([a-z])([A-Z])", "$1 $2");
                        return new[] { controllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                c.DocInclusionPredicate((name, api) => true);
            });


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<Business.Interfaces.IAuthorizationService, AuthorizationService>();
            services.AddScoped<IStationManagementService, StationManagementService>();
            services.AddScoped<IVehicleTypeService, VehicleTypeService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IPartnerManagementService, PartnerManagementService>();
            services.AddScoped<ICustomerManagementService, CustomerManagementService>();
            services.AddScoped<IRentStationManagementService, RentStationManagementService>();
            services.AddScoped<IPurchaseHistoryService, PurchaseHistoryService>();
            services.AddScoped<IServiceTypeManagementService, ServiceTypeManagementService>();
            services.AddScoped<ICustomerPackagesHistoryService, CustomerPackagesHistoryService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IRouteManagementService, RouteManagementService>();
            services.AddScoped<IPriceBusServiceConfig, PriceBusServiceConfigService>();
            services.AddScoped<ICategoryManagementService, CategoryManagementService>();
            services.AddScoped<IPublishYearManagementService, PublishYearManagementService>();
            services.AddScoped<IPriceBookingServiceConfig, PriceBookingServiceConfigService>();
            services.AddScoped<IPriceRentingServiceConfig, PriceRentingServiceConfigService>();
            services.AddScoped<ICardManagementService, CardManagementService>();
            services.AddScoped<IVehicleCollectionService, VehicleCollectionService>();
            services.AddScoped<IVehicleManagementService, VehicleManagementService>();
            services.AddScoped<IDriverManagementService, DriverManagementService>();
            services.AddScoped<IOrderHelpersService, OrderHelpersService>();
            services.AddScoped<ITripManagementService, TripManagementService>();
            services.AddScoped<IDepositService, DepositService>();
            services.AddScoped<IPowerBIService, PowerBIService>();

            //Azure AD
            services.AddScoped(_ =>
            {
                var model = new AzureViewModel();
                model.ClientId = Configuration.GetSection("PowerBI").GetSection("clientId").Value;
                model.ClientSecret = Configuration.GetSection("PowerBI").GetSection("clientSecret").Value;
                model.GrantType = Configuration.GetSection("PowerBI").GetSection("grantType").Value;
                model.Password = Configuration.GetSection("PowerBI").GetSection("password").Value;
                model.Username = Configuration.GetSection("PowerBI").GetSection("username").Value;
                model.Resource = Configuration.GetSection("PowerBI").GetSection("resource").Value;
                model.DataSetId = Configuration.GetSection("PowerBI").GetSection("datasetId").Value;
                model.ReportId = Configuration.GetSection("PowerBI").GetSection("reportId").Value;
                return model;
            });

            // Azure blob
            services.AddScoped(_ => new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage")));

            // Custom Error Message for Model Validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext => new CustomErrorResponse().ErrorResponse(actionContext);
            });

            // SMS
            var sms = Configuration.GetSection("SMS");
            services.AddScoped(_ => Credentials.FromApiKeyAndSecret(sms.GetSection("SMS_API_KEY").Value, sms.GetSection("SMS_API_Secret").Value));

            // Email
            services.AddScoped(_ =>
            {
                var client = new HttpClient() { BaseAddress = new Uri(Configuration.GetSection("SendEmailFunction").GetSection("Uri").Value) };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-functions-key", Configuration.GetSection("SendEmailFunction").GetSection("Key").Value);
                return client;
            });

            // Twilio setting
            services.AddSingleton<ITwilioSettings, TwilioSettings>(_ =>
              Configuration
                  .GetSection(nameof(TwilioSettings))
                  .Get<TwilioSettings>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/admin/swagger.json", "TourismSmartTransportation.API Admin");
                c.SwaggerEndpoint("/swagger/partner/swagger.json", "TourismSmartTransportation.API Partner");
                c.SwaggerEndpoint("/swagger/driver/swagger.json", "TourismSmartTransportation.API Driver");
                c.SwaggerEndpoint("/swagger/customer/swagger.json", "TourismSmartTransportation.API Customer");
                c.RoutePrefix = "";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            // Handle Exceptions
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;
                var response = new
                {
                    statusCode = 500,
                    message = $"Lỗi hệ thống: {exception.Message}"
                };
                await context.Response.WriteAsJsonAsync(response);
            }));

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                if (env.IsDevelopment())
                    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                else
                    endpoints.MapControllers();
            });
        }
    }
}
