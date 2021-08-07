using ASP.NET_API.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NET_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ASP.NET_API.Helper;

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                sqlServer => sqlServer.UseNetTopologySuite())); //activar querys espaciales
            //SQL SERVER CONEXION
            services.AddCors(); //agregar Cors sin policies

            //para agregar Policies Cors
            /*   services.AddCors(opt =>
               {
                   opt.AddPolicy("AllowApiRequestIO", builder =>
                   builder.WithOrigins("url")
                   .WithMethods("GET", "POST")
                   .AllowAnyHeader());
               });*/ //y poner [EnableCors(PolicyName = "AllowApiRequestIO")] en los metodos que quiera permitir

            //agregar encriptacion
            services.AddDataProtection();

            services.AddAutoMapper(typeof(Startup)); //AGREGAR AUTOmappe

            services.AddTransient<HashService>();
            //Agregar HashService
            services.AddTransient<IFileStorageService, InAppStorageService>(); //servicio de guardado de img
            services.AddTransient<IHostedService, MovieInTheatersService>(); //Servicio de Cambiado automatico a true En InTheater
            

            services.AddIdentity<IdentityUser,IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //identity servicio (login)

            /*HATEOAS GENERATE LINKS
             */
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<GenreHATEOASAttribute>();
            services.AddTransient<LinksGenerator>();
            

            //jwt y validando 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                    opt.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        //ValidIssuer = "tudomain", activar arriba a true issuer y audience
                        //ValidAudience = "tudomain",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["jwt:key"]) //configurar el provedor
                            ),
                        ClockSkew = TimeSpan.Zero //verificar si el token ha expirado
                    }
                );

            services.AddHttpContextAccessor(); //para guardar img


            services.AddControllers()
                
                .AddXmlDataContractSerializerFormatters();//admitir formato xml en metodos 
                                                          //para ver xml en postman en header poner variable Accept y application/xml 


            //sirve para configurar Swashbuckle.AspNetCore que se usa para documentar la api
            /*para agregar comentarios pegarlo en el csproj
             * <PropertyGroup>
                    <GenerateDocumentationFile>true</GenerateDocumentationFile>
                    <NoWarn>$(NoWarn);1591</NoWarn>
                </PropertyGroup>*/
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies_API", Version = "v1"
                ,Description = "Esta Api es Peliculas",
                Contact= new OpenApiContact
                {
                    Name= "David Hernandez Juajinoy",
                    Email = "davidhernandezjuajinoy@gmail.com",
                    Url= new Uri("https://github.com/Davidjuajinoy")
                }
                }); 
                //para agregar mas comentarios debe introducirse el PropertyGroup y los comentarios que se mostraran son el summary
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory,xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


                  



      


            //Inyeccion de dependencias
            
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
                app.UseSwagger(); //habilitar swagger
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies_API")); /*Para que pueda mirar los endpoints*/
            }

            app.UseHttpsRedirection();

            //para guardar archivos en wwwroot
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //para agregar policies arriba en services
            /*app.UseCors();*/
            app.UseCors(builder =>
                builder.WithOrigins("https://www.apirequest.io")
                .WithMethods("GET", "POST")
                .AllowAnyHeader()
            );
            //para agregar a todos los metodos

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
