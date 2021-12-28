using Boxed.Mapping;
using InventoryManagement.Api.Mapper;
using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Domain.Services;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Sieve.Services;
using System.Text;
using System;
using System.Net.Http;
using InventoryManagement.Api.Authentication;

namespace InventoryManagement.Api
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
                Configuration.GetConnectionString("ApplicationDbContext")
                ));

            // Unit of work
            services.AddScoped<UnitOfWork>();
            
            // Services
            services.AddScoped<InventoryService>();
            services.AddScoped<OutboxEventService>();
            services.AddScoped<WarehouseService>();
            services.AddScoped<PartService>();

            // Mapper
            services.AddMappers();


            // CORS
            services.AddCors(option => 
                option.AddPolicy(
                    CorsPolicyName.AllowAny,
                    x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                )
            );

            // Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt => {
                var key = Encoding.ASCII.GetBytes(Configuration["Authentication:Secret"]);
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            })
            .AddApiKey(options => { });

            services.AddScoped<GetApiKeyFromConfig>();

            // Client Masterdata
            services.AddHttpClient<IMasterClient, MasterClient>(client => {
                client.BaseAddress = new Uri(Configuration["External:Masterdata:BaseUrl"]);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            })
            .AddPolicyHandler(RetryPolicy.GetRetryPolicy(Configuration))
            .AddPolicyHandler(CircuitBreakerPolicy.GetCircuitBreakerPolicy(Configuration));

            services.AddHttpContextAccessor();
            services.AddHealthChecks();
            services.AddScoped<ISieveProcessor, SieveProcessor>();
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventoryManagement.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            // migrate any database changes on startup (includes initial db creation)
            //context.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryManagement.Api v1"));
            }

            app.UseSerilogRequestLogging();

            app.UseMiddleware<GlobalHandlerException>();

            app.UseRouting();
            app.UseCors(CorsPolicyName.AllowAny);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthcheck").RequireCors(CorsPolicyName.AllowAny);
                endpoints.MapControllers().RequireCors(CorsPolicyName.AllowAny);
            });
        }
    }
}
