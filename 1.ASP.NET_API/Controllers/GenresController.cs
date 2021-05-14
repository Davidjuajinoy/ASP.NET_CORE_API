using ASP.NET_API.Entities;
using ASP.NET_API.Filters;
using ASP.NET_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly ILogger<GenresController> logger;

        public GenresController(IRepository repository, ILogger<GenresController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        //tiene dos endpoint api/Genres/list y api/Genres
        [HttpGet()]
        [HttpGet("list")]
        [ResponseCache(Duration =60)]  //guarda el metodo por 60s en cache para mirar en postman tiene que desactivar send no cache header y si se va a postman en la respuesta en header se encuantra el age que es el tiempo q tendra el cache osea 60
        //agregar metodo al cache
        //[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)] //activar seguridad jwt

        [ServiceFilter(typeof(MyActionFilter))] //usar filtro custom y hay que agregar en startup el filtro 
        public ActionResult<List<Genre>> Get()
        {
            logger.LogDebug("David trajo la lista");
            return repository.GetAllGenres();
        }

        [HttpGet("{id:int}", Name ="GetGenre")] //es lo mismo  si se pone / antes se borra la ruta principal de arriba
        //[Route("{id:int}")]
        //[HttpGet("{id = 1}")] tambien se puede definir el dato de una vez
        public IActionResult Get(int id )
        {
            var genre =  repository.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            return Ok(genre);

        }


        [HttpPost]
        public IActionResult Post([FromBody] Genre genre)
        {
            repository.AddGenre(genre);

            return new CreatedAtRouteResult("GetGenre", new { id = genre.Id  },genre);
        }

    }
}
