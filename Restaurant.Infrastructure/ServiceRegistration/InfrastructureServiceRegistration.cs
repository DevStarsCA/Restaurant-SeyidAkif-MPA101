using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Services;
using Restaurant.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.ServiceRegistration
{
    
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Identity
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.Zero
                };

                // SignalR üçün token
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs/order") || path.StartsWithSegments("/hubs/chat")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // SignalR
            services.AddSignalR();

            // HttpClient - Kapital Bank
            services.AddHttpClient("KapitalBank", client =>
            {
                client.BaseAddress = new Uri(configuration["KapitalBank:BaseUrl"] ?? "https://e-commerce.kapitalbank.az");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IKapitalBankService, KapitalBankService>();
            services.AddScoped<IQRCodeService, QRCodeService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAiAssistantService, AiAssistantService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }

}
