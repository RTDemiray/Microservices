using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.Shared.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace FreeCourse.Service.FakePayment
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
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["RabbitMQUrl"],"/", host =>
                    {
                        //Default port 5672 olduğu için belirtmedik.
                        host.Username("guest");
                        host.Password("guest");
                    });
                });
            });
            services.AddMassTransitHostedService();
            
            services.AddHttpContextAccessor();
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub"); //Sub key'ini mapleme.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = Configuration["IdentityServerURL"];
                options.Audience = "resource_fakepayment";
                options.RequireHttpsMetadata = false;
            });
            
            services.AddControllers(opt => opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy)));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FreeCourse.Service.FakePayment", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(
                    c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Service.FakePayment v1"));
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}