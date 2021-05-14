using ASP.NET_API.Filters;
using ASP.NET_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_API
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

            services.AddControllers()
                
                .AddXmlDataContractSerializerFormatters();//admitir formato xml en metodos 
                                                          //para ver xml en postman en header poner variable Accept y application/xml 

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ASP.NET_API", Version = "v1" });
            });



            services.AddResponseCaching(); //Activar servicios de cache
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); // jwt prueba hay que activar useAuthentication

            services.AddTransient<MyActionFilter>();//Ejecutar el filtro personalizado 

            //Inyeccion de dependencias
            services.AddSingleton<IRepository,InMemoryRepository>();
            //services.AddSingleton sirve para  q siempre sea la misma instancia en todo el contexto

            //services.AddTransient las instancias siempre son diferentes; se proporciona una nueva instancia a todos los controladores y a todos los servicios.

            //services.AddScoped sirve para que al realizar un cambio por otra clase en la misma solicitud (http) trae los cambios hechos , pero solo en la misma instancia. los objetos son los mismos dentro de una solicitud, pero diferentes en diferentes solicitudes.

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET_API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseResponseCaching(); //para usar el cache 

            app.UseAuthentication(); //jwt

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
