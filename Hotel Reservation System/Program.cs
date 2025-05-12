using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using HRS_Presentation.Middlewares;
using Hangfire;
using HRS_DataAccess.Models;
using HRS_BussinessLogic.Models;
using BussinessLogic;
using HRS_DataAccess;
using HRS_ServiceLayer.IServices;
using HRS_ServiceLayer.Services.Reservations;
using Stripe;
using Scalar.AspNetCore;
using QuestPDF.Infrastructure;
using HRS_ServiceLayer.Services.Offers;
using HRS_SharedLayer.Interfaces.IBases;
using HRS_BussinessLogic.Validators;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;
namespace HRS.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddLogging();
            builder.Services.AddProblemDetails();
            builder.Services.AddHangfire(config => 
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("cs")));
            builder.Services.AddHangfireServer();
            #region Dependency Injection    
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient(typeof(IBaseValidator<>),typeof(BaseValidator<>));

            builder.Services.Scan(s => s
                    .FromAssemblyOf<IUserService>()
                        .AddClasses(c => c.Where(type => type.Name.EndsWith("Service")))
                            .AsImplementedInterfaces()
                                .WithScopedLifetime());
            QuestPDF.Settings.License = LicenseType.Community;
            #endregion
            // ----------------------------------------------------------------------------
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            builder.Services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
            builder.Services.AddAuthentication(options =>
            { // because the default is Cookie
                // Check JWT Token Header
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // returns UnAuthorized
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => // Check Verified Token
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:IssuerIP"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:AudienceIP"],
                    IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
                };
            });
            // ----------------------------------------------------------------------------
            builder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("cs"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 8 Web API",
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme{Reference = new OpenApiReference
                        {Type = ReferenceType.SecurityScheme,Id = "Bearer"}},new string[] {}
                    }
                });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            var app = builder.Build();
            var recurringJob = app.Services.GetRequiredService<IRecurringJobManager>();
            recurringJob.AddOrUpdate<ReservationCleanupService>("ReservatinCleanup",
                x => x.MarkExpiredReservationsAsync(), Cron.Hourly(55));
            recurringJob.AddOrUpdate<ReservationStatusService>("UpdateRoomAvailability",
                x => x.UpdateRoomAvailabilityAsync(), Cron.Hourly(55));
            recurringJob.AddOrUpdate<OfferCleanupService>("OfferCleanup",
                x => x.MarkExpiredOffers(), Cron.Hourly(55));

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.MapOpenApi();
                app.MapScalarApiReference();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();

            app.UseMiddleware<LoggingMiddleware>();
            app.UseCors("MyPolicy");
            app.UseAuthorization();
            app.UseHangfireDashboard("/dashboard");
            app.UseExceptionHandler();
            app.UseStatusCodePages();
            app.MapControllers();

            app.Run();
        }
    }
}
