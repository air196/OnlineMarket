using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineMarket.Configuration;
using OnlineMarket.DAL;
using OnlineMarket.Services;
using System.Reflection;
using System.Text;

namespace OnlineMarket
{
    public static class Startup
    {
        private static string ProjectName => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static Task<WebApplicationBuilder> ConfigureServices(this WebApplicationBuilder builder)
        {
            DataSourceLoadOptionsBase.StringToLowerDefault = true;

            builder
                .AddDbContexts()
                .AddSwagger()
                .AddControllers()
                .ConfigureJwtSettings()
                .AddServices()
                .AddBearerAuthentication()
                .AddAuthorization();

            return Task.FromResult(builder);
        }

        private static WebApplicationBuilder ConfigureJwtSettings(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            return builder;
        }

        private static WebApplicationBuilder AddDbContexts(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("shop");

            builder.Services.AddDbContext<MarketDbContext>(config =>
            {
                config.UseNpgsql(connectionString, builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
                });
            },
                ServiceLifetime.Singleton);

            return builder;
        }

        private static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = ProjectName,
                    Version = "v1"
                });

                c.CustomSchemaIds(type => type.Name);

                var filePath = Path.Combine(AppContext.BaseDirectory, $"{ProjectName}.xml");
                if (File.Exists(filePath))
                    c.IncludeXmlComments(filePath);
            });

            return builder;
        }

        private static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            return builder;
        }

        private static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<OnlineMarketService>();
            return builder;
        }

        private static WebApplicationBuilder AddBearerAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddAuthentication(authOptions =>
                {
                    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration.GetRequiredSection("JwtSettings:Key").Value);

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            return builder;
        }

        private static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization();
            return builder;
        }
    }
}
