using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TourismSmartTransportation.API.Utilities.Response;
using TourismSmartTransportation.API.Utilities.Swagger;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Implements;
using TourismSmartTransportation.Business.Implements.Admin;
using TourismSmartTransportation.Business.Implements.Company;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Company;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Repositories;

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
            services.AddDbContext<TourismSmartTransportationContext>(
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

            services.AddScoped<NotAllowedNullPropertiesAttribute>();

            services.AddControllers();

            services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("admin", new OpenApiInfo { Title = "TourismSmartTransportation.API Admin", Version = "admin" });
                c.SwaggerDoc("company", new OpenApiInfo { Title = "TourismSmartTransportation.API Company", Version = "company" });
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
            services.AddScoped<ICompanyManagementService, CompanyManagementService>();
            services.AddScoped<IServiceManagementService, ServiceManagement>();
            services.AddScoped<ICustomerManagementService, CustomerManagementService>();
            services.AddScoped<IRentStationManagementService, RentStationManagementService>();

            // Azure blob
            services.AddScoped(_ => new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage")));

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
                c.SwaggerEndpoint("/swagger/company/swagger.json", "TourismSmartTransportation.API Company");
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
                var response = new ErrorModel() { Error = exception.Message };
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
