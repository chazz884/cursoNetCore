using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;

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
            
            // se puede injectar la interfaz e instanciar la clase
            // services.AddTransient<ServicioA>(); se puede injectar la clase de la interfaz directamente 
            // AddScope = cambia la instancia
            // AddSingleton = se usa siempre la misma instancia
            services.AddTransient<IServicio,  ServicioA>(); 

            services.AddTransient<ServicioTransient>(); 
            services.AddScoped<ServicioScope>(); 
            services.AddSingleton<ServicioSingleton>(); 

            services.AddHostedService<EscribirEnArchivo>();
            services.AddTransient<MiFiltroDeAccion>();

            services.AddResponseCaching();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebAPIAutores", Version = "v1"}));

        }
        // Configure se usan los middleweare
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // app.UseMiddleware<LoguearRespuestaHTTPMiddleware>(); Se expone directamente  la clase quee usamos para el middleware
            app.UseLoguearRespuestaHTTP(); 
            // Tubería con ruta especific para que no interfiera con las demas
            app.Map("/ruta1", app =>
            {
                app.Run(async contexto => {
                    await contexto.Response.WriteAsync("Estoy en la tubería de primeras");
                });   
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

