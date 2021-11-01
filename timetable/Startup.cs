using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using timetable.Data;
using timetable.Controllers;
using timetable.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace timetable
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
            services.AddScoped<DataContext, DataContext>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "timetable", Version = "v1" });
            });
            
            // Prepare App-Settings
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // GET Key for jwt-token creation
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            // Configure jwt authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                Console.Write("HERE_0\n");
                x.Events = new JwtBearerEvents
                {

                    OnTokenValidated = context =>
                    {
                        // Debug
                        Console.Write("HERE_1\n");

                        // Magic
                        string token = context.Request.Headers["Authorization"];
                        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            token = token.Substring("Bearer ".Length).Trim();
                        }
                        // Console.WriteLine(token);
                        
                        var jwtTokenHandler = new JwtSecurityTokenHandler();
                        var principal = jwtTokenHandler.ValidateToken(
                            token,
                            new TokenValidationParameters
                            {
                                // Check Issuer
                                ValidateIssuer = false,
                                // ValidIssuer = "",

                                // Check Audience
                                ValidateAudience = false,
                                // ValidAudience = "",
                                ValidateLifetime = true,

                                // Set Security-Key
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ValidateIssuerSigningKey = true,

                                //
                                ClockSkew = TimeSpan.Zero
                            },
                            out var validatedToken
                        );

                        // Now we need to check if the token has a valid security algorithm
                        if(validatedToken is JwtSecurityToken jwtSecurityToken)
                        {
                            var result = jwtSecurityToken.Header.Alg.Equals(
                                SecurityAlgorithms.HmacSha256,
                                StringComparison.InvariantCultureIgnoreCase
                            );

                            if(result == false)
                            {
                                Console.Write("HERE_2\n");
                                context.Fail("Unauthorized");
                            }
                            Console.Write("HERE_3\n");
                        }

                        // Will get the time stamp in unix time
                        var utcExpiryDate = long.Parse(
                            principal.Claims.FirstOrDefault(
                                x => x.Type == JwtRegisteredClaimNames.Exp
                            ).Value
                        );

                        // we convert the expiry date from seconds to the date
                        var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                        if( expDate < DateTime.UtcNow )
                        {
                            Console.Write("HERE_5\n");
                            context.Fail("Unauthorized");
                            return Task.CompletedTask;
                        }

                        Console.Write("HERE_4\n");
                        context.Success();

                        return Task.CompletedTask;
                    }
                };
                x.SaveToken = true;

                // NOTE: means 'IsUseHttps'
                x.RequireHttpsMetadata = false;
                
                // Set Token Parameters
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    // Check Issuer
                    ValidateIssuer = false,
                    // ValidIssuer = "",

                    // Check Audience
                    ValidateAudience = false,
                    // ValidAudience = "",
                    ValidateLifetime = true,

                    // Set Security-Key
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,

                    //
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "timetable v1"));
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()
            ); // allow credentials

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
