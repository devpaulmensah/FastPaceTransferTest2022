using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using FastPaceTransferTest2022.Api.Configurations;
using FastPaceTransferTest2022.Api.Database;
using FastPaceTransferTest2022.Api.Helpers;
using FastPaceTransferTest2022.Api.Models.Responses;
using FastPaceTransferTest2022.Api.ServiceExtensions;
using FastPaceTransferTest2022.Api.Services.Interfaces;
using FastPaceTransferTest2022.Api.Services.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FastPaceTransferTest2022.Api
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
            services.AddControllers();
            
            services
                .AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseMySql(Configuration.GetConnectionString("DbConnection"));
                    }
                    , ServiceLifetime.Transient).AddUnitOfWork<ApplicationDbContext>();
            
            services.AddAuthentication(x =>
                    {
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = async context =>
                            {
                                await Task.Delay(0);
                                
                                var user = context.Principal.FindFirst(c => c.Type == ClaimTypes.Thumbprint).Value;
                                var userData = JsonConvert.DeserializeObject<UserResponse>(user);
                                
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Thumbprint, JsonConvert.SerializeObject(userData)),
                                };
                                
                                var appIdentity = new ClaimsIdentity(claims, CommonConstants.AppAuthIdentity);
                                context.Principal.AddIdentity(appIdentity);
                            }
                        };
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidIssuer = Configuration["BearerTokenConfig:Issuer"],
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["BearerTokenConfig:Key"])),
                            ValidAudience = Configuration["BearerTokenConfig:Audience"],
                        };
                    });

            services.InitializeRedis(new RedisConfiguration
            {
                Url = Configuration["RedisConfiguration:Url"]
            });
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IOtpService, OtpService>();
            
            services.Configure<BearerTokenConfig>(Configuration.GetSection("BearerTokenConfig"));
            services.Configure<EmailConfiguration>(Configuration.GetSection("EmailConfiguration"));
            
            services.AddAutoMapper(typeof(Startup));
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "FastPaceTransfer2022 Api"
                });
                
                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Provide bearer token to access endpoints",
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
                            Scheme = "oauth2",
                            Name = "Bearer",
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.AddCors();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FastPaceTransfer Api");
            });
            
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}