using AutoMapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mobi.Repository;
using Mobi.Repository.Migrations;
using Mobi.Service.Employees;
using Mobi.Service.Factories;
using Mobi.Service.Helpers;
using Mobi.Service.Locations;
using Mobi.Service.SystemUser;
using Mobi.Web.Areas.Admin.Utilities;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Factories.Locations;
using Mobi.Web.Utilities;
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
            // Add Controllers with Views
            services.AddControllersWithViews(options =>
            {
                // Apply the Web Policy (Cookie) to all MVC controllers globally
                //options.Filters.Add(new AuthorizeFilter("WebPolicy"));
            }).AddRazorRuntimeCompilation();

            services.AddControllers(options =>
            {
                // Apply the Api Policy (JWT) to all API controllers globally
                //options.Filters.Add(new AuthorizeFilter("ApiPolicy"));
            });

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

            // Add JWT Authentication
            var jwtSettings = Configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSettings);


            // Add Swagger for API Documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mobi API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;   
            })
             .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Redirect to login page
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            //services.AddAuthorization(options =>
            //{
            //    // Web Policy for Cookie Authentication (applied globally to MVC controllers)
            //    options.AddPolicy("WebPolicy", policy =>
            //    {
            //        policy.RequireAuthenticatedUser()
            //              .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
            //    });

            //    // API Policy for JWT Authentication (applied globally to API controllers)
            //    options.AddPolicy("ApiPolicy", policy =>
            //    {
            //        policy.RequireAuthenticatedUser()
            //              .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            //    });
            //});



            // Register Repository and Services
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ISystemUserService, SystemUserService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ILocationService, LocationService>();

            // Register Factories
            services.AddScoped<ISystemUserFactory, SystemUserFactory>();
            services.AddScoped<IEmployeeFactory, EmployeeFactory>();
            services.AddScoped<ILocationFactory, LocationFactory>();


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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

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

            app.UseAuthentication(); // Enables Authentication
            app.UseAuthorization();  // Enables Authorization

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
