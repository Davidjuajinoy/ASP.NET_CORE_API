using ASP.NET_API.DTOs;
using ASP.NET_API.Entities;
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
       
        [HttpGet()]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            //instalar automapper
            var genres= await context.Genres.ToListAsync();

            var genresDto = mapper.Map<List<GenreDTO>>(genres);

            return genresDto;

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
        [HttpGet("{id:int}", Name ="GetGenre")] //es lo mismo  si se pone / antes se borra la ruta principal de arriba
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
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Genre genre)
        {
            context.Add(genre);
            await context.SaveChangesAsync();
            var genreDTO = mapper.Map<GenreDTO>(genre);
            return new CreatedAtRouteResult("GetGenre", new { id = genreDTO.Id  }, genreDTO);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
        [HttpPut("{Id:int}")]
        public async Task<IActionResult> Put(int Id, [FromBody] GenreDTO genreDTO)
        {
            var genre = mapper.Map<Genre>(genreDTO);
            genre.Id = Id;
            context.Entry(genre).State = EntityState.Modified;
       
            await context.SaveChangesAsync();
            return new CreatedAtRouteResult("GetGenre", new { id = genre.Id }, genre);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{Id:int}")]
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
