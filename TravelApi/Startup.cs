
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models.Notification;
using Travel.Context.Models.Travel;
using Travel.Data.Interfaces;
using Travel.Data.Repositories;
using TravelApi.Extensions;
using TravelApi.Hubs;

namespace TravelApi
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSignalR(e => {
                e.EnableDetailedErrors = true;
                e.MaximumReceiveMessageSize = 102400000;
            }); services.AddCors(options => {
                options.AddPolicy("CORSPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TravelApi", Version = "v1" });
            });

            services.AddDatabase(Configuration)
                .AddRepositories();
            services.AddScoped<IVoucher, VoucherRes>();
            services.AddDbContext<TravelContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("travelRoverEntities")));

            //services.AddDbContext<NotificationContext>(options =>
            //        options.UseSqlServer(Configuration.GetConnectionString("notifyTravelEntities")));

            services.AddControllersWithViews()
                 .AddNewtonsoftJson(options =>
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Token:Audience"],
                    ValidIssuer = Configuration["Token:Issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(Convert.ToInt16(Configuration["Token:TimeExpired"])),
                    //ClockSkew = TimeSpan.FromMinutes(525600),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"])),
             
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });


















            // add automapper
            Travel.Shared.Ultilities.Mapper.RegisterMappings();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
           
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelApi v1"));
            

            app.UseHttpsRedirection();

            app.UseRouting();
           
            app.UseCors(x => x
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true) // allow any origin
               .AllowCredentials());
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();


            // signal R
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<TravelHub>("/travelhub");

            });



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            // add
            Configs.GetConfigItems.HttpContextAccessor = httpContextAccessor;
            Configs.GetConfigItems.WebHostEnvironment = env;
        }
    }
}
