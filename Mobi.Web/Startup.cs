using AutoMapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mobi.Repository;
using Mobi.Repository.Migrations;
using Mobi.Service.Compnay;
using Mobi.Service.EmployeeAttendances;
using Mobi.Service.EmployeeLocationServices;
using Mobi.Service.Employees;
using Mobi.Service.Factories;
using Mobi.Service.Helpers;
using Mobi.Service.LocationBeaconMappings;
using Mobi.Service.LocationBeacons;
using Mobi.Service.Locations;
using Mobi.Service.Pictures;
using Mobi.Service.ResourceService;
using Mobi.Service.SystemUser;
using Mobi.Web.Areas.Admin.Factories;
using Mobi.Web.Areas.Admin.Utilities;
using Mobi.Web.Factories.EmployeeLocations;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Factories.LocationBeacons;
using Mobi.Web.Factories.Locations;
using Mobi.Web.Utilities;
using Mobi.Web.Utilities.Filters;
using System.Text;

namespace Mobi.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add Controllers with Views (Web Application)
            //services.AddControllersWithViews(options =>
            //{
            //    options.Filters.Add(new AuthorizeFilter("WebPolicy"));
            //}).AddRazorRuntimeCompilation();

            //// Add API Controllers (Admin API)
            //services.AddControllers(options =>
            //{
            //    options.Filters.Add(new AuthorizeFilter("ApiPolicy"));
            //});
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            // Configure Kestrel's request limits
            services.Configure<KestrelServerOptions>(Configuration.GetSection("Kestrel"));

            // Add EF Core with SQL Server
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Configure FluentMigrator
            services.AddFluentMigratorCore()
                .ConfigureRunner(runner => runner
                    .AddSqlServer()
                    .WithGlobalConnectionString(Configuration.GetConnectionString("DefaultConnection"))
                    .ScanIn(typeof(SchemaMigration).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            // Add AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

            // JWT Authentication Configuration
            var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddSingleton(jwtSettings);
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));


            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            services.AddAuthentication(options =>
            {
                // Default schemes for both authentication systems
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Default for MVC
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  // Default for API
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                // Cookie settings for the MVC app
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
                var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        if (string.IsNullOrEmpty(token))
                        {
                            Console.WriteLine("No token provided in request.");
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully.");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("{\"error\":\"Unauthorized - Invalid or missing JWT token.\"}");
                    }
                };
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Mobi API",
                    Version = "v1"
                });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token."
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Ensure Swagger doesn't redirect to the login page for unauthorized responses
                c.OperationFilter<RemoveUnauthorizedRedirectFilter>();
            });



            // Authorization Policies
            services.AddAuthorization(options =>
            {
                // Web Policy for MVC using Cookie Authentication
                options.AddPolicy("WebPolicy", policy =>
                    policy.RequireAuthenticatedUser()
                          .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme));

                // API Policy for JWT Authentication
                options.AddPolicy("ApiPolicy", policy =>
                    policy.RequireAuthenticatedUser()
                          .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
            });

            // Register Repositories and Services
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ISystemUserService, SystemUserService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<ILocationBeaconService, LocationBeaconService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IPictureService, PictureService>();
            services.AddScoped<IEmployeeLocationService, EmployeeLocationService>();
            services.AddScoped<IEmployeeAttendanceService, EmployeeAttendanceService>();
            

            // Register Factories
            services.AddScoped<ISystemUserFactory, SystemUserFactory>();
            services.AddScoped<IEmployeeFactory, EmployeeFactory>();
            services.AddScoped<ILocationFactory, LocationFactory>();
            services.AddScoped<ILocationBeaconFactory, LocationBeaconFactory>();
            services.AddScoped<IBeaconLocationFactory, BeaconLocationFactory>();
            services.AddScoped<IEmployeeLocationsFactory, EmployeeLocationsFactory>();
            
            // Add memory cache service
            services.AddMemoryCache();
            // Register Helpers
            services.AddSingleton<JwtTokenHelper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mobi API V1");
                    c.RoutePrefix = "swagger";
                });
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mobi API V1");
                    c.RoutePrefix = "swagger";
                });
                //app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Apply FluentMigrator Migrations
            //using (var scope = app.ApplicationServices.CreateScope())
            //{
            //    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            //    runner.MigrateUp();
            //}

            // Apply FluentMigrator Migrations
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                try
                {
                    if (dbContext.Database.EnsureCreated())
                    {
                        runner.MigrateUp();
                        Console.WriteLine("Database created and Migrations applied successfully.");
                    }
                    else
                    {
                        runner.MigrateUp();
                        Console.WriteLine("Migrations applied successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during migration: {ex.Message}");
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllers(); // For API endpoints
            });
        }
    }
}
