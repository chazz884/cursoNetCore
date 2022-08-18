using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            }).AddJsonOptions(x => 
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
            );
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebAPIAutores", Version = "v1"}));

            services.AddAutoMapper(typeof(Startup));

        }
        // Configure se usan los middleweare
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseLoguearRespuestaHTTP(); 

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

