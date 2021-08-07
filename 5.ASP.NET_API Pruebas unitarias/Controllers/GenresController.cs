using ASP.NET_API.DTOs;
using ASP.NET_API.Entities;
using ASP.NET_API.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)] //requerir JWT en TODA LA CLASE
    public class GenresController : ControllerBase
    {
       
        private readonly ILogger<GenresController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController( ILogger<GenresController> logger, ApplicationDbContext context, IMapper mapper)
        {
         
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        //tiene dos endpoint api/Genres/list y api/Genres

        /// <summary>
        /// Mostrar lista de Generos de Peliculas
        /// </summary>
        /// <returns>Retorna una lista de Generos de Peliculas</returns>
        //[ProducesResponseType(404)] //son para el swagger
        //[ProducesResponseType(typeof(GenreDTO),200)]//son para el swagger
        //[HttpGet(Name ="getGenres")]
    /*   HATEOAS enviado como parametro
     *   public async Task<IActionResult> Get(bool includeHATEOAS = true)
        {
            //instalar automapper
            var genres= await context.Genres.ToListAsync();

            var genresDto = mapper.Map<List<GenreDTO>>(genres);


            if(includeHATEOAS)
            {
                var resourceCollection = new ResourceCollection<GenreDTO>(genresDto);
                genresDto.ForEach(genre => GenerateLinks(genre));
                resourceCollection.Links.Add(new Link(Url.Link("getGenres", new {}), "get-genres", method: "GET"));
                resourceCollection.Links.Add(new Link(Url.Link("createGenre", new {}), "create-genre", method: "DELETE"));
                return Ok(resourceCollection);
            }

            return Ok(genresDto);

        }*/

        [ProducesResponseType(404)] //son para el swagger
        [ProducesResponseType(typeof(GenreDTO), 200)]//son para el swagger
        [HttpGet(Name = "getGenres")]
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        public async Task<IActionResult> Get()
        {
            //instalar automapper
            var genres = await context.Genres.ToListAsync();
            var genresDto = mapper.Map<List<GenreDTO>>(genres);
            return Ok(genresDto);

        }

        /* HateOs
         * Links de editar y eliminar Genres Dto*/
        private void GenerateLinks(GenreDTO genreDTO)
        {
            genreDTO.Links.Add(new Link(Url.Link("GetGenre", new { id= genreDTO.Id }),"get-genre" ,method:"GET"));
            genreDTO.Links.Add(new Link(Url.Link("putGenre",new { Id= genreDTO.Id }),"put-genre" ,method:"PUT"));
            genreDTO.Links.Add(new Link(Url.Link("deleteGenre",new { Id= genreDTO.Id }),"delete-genre" ,method:"DELETE"));
        }


        [HttpGet("{id:int}", Name ="GetGenre")] //es lo mismo  si se pone / antes se borra la ruta principal de arriba
        [ServiceFilter(typeof(GenreHATEOASAttribute))] //llamar el filtro de genre y para mirar poner includeHATEOAS=y  en  el header
        public async Task<ActionResult<GenreDTO>> Get(int id )
        {
            var genre = await  context.Genres.FirstOrDefaultAsync(g => g.Id==id);

            var genreDTO = mapper.Map<GenreDTO>(genre);

            if (genre == null)
            {
                return NotFound();
            }

            return genreDTO;

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
        [HttpPost(Name ="createGenre")]
        public async Task<IActionResult> Post([FromBody] Genre genre)
        {
            context.Add(genre);
            await context.SaveChangesAsync();
            var genreDTO = mapper.Map<GenreDTO>(genre);
            return new CreatedAtRouteResult("GetGenre", new { id = genreDTO.Id  }, genreDTO);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
        [HttpPut("{Id:int}",Name = "putGenre")]
        public async Task<IActionResult> Put(int Id, [FromBody] GenreDTO genreDTO)
        {
            var genre = mapper.Map<Genre>(genreDTO);
            genre.Id = Id;
            context.Entry(genre).State = EntityState.Modified;
       
            await context.SaveChangesAsync();
            return new CreatedAtRouteResult("GetGenre", new { id = genre.Id }, genre);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{Id:int}",Name = "deleteGenre")]
        public async Task<ActionResult> Delete(int Id)
        {
            var exist = await context.Genres.AnyAsync(x => x.Id == Id);
            if (exist)
            {
                context.Remove(new Genre { Id = Id});
                await context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}
