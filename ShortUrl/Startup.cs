using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShortUrl.Repository;
using ShortUrl.Repository.Implementations;
using ShortUrl.Repository.Interfaces;
using ShortUrl.Service.Helpers;
using ShortUrl.Service.Implementations;
using ShortUrl.Service.Interfaces;
using Swashbuckle.AspNetCore.Swagger;

namespace ShortUrl
{
    public class Startup
    {
        private IConfiguration _config { get; }

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseSqlServer(_config.GetConnectionString("ShortUlrDbConnection")));

            services.AddScoped<IShortenerService, ShortenerService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddSingleton(new ServiceHelper(_config));
            services.AddScoped<IUrlInfoRepository, UrlInfoRepository>();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1.0", new Info {Title = "ShortURL API", Version = "1.0"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "ShortURL API (V 1.0)"); });

            app.UseMvc();
        }
    }
}
