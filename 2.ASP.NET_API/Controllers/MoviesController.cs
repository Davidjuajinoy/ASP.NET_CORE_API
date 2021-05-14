using ASP.NET_API.DTOs;
using ASP.NET_API.DTOs.Movie;
using ASP.NET_API.Entities;
using ASP.NET_API.Helper;
using ASP.NET_API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly ILogger<MoviesController> logger;
        private readonly string containerName = "Movies";

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService, ILogger<MoviesController> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            this.logger = logger;
        }

        // GET: api/<MoviesController>
        [HttpGet]
        public async Task<ActionResult<IndexMoviePageDTO>> Get()
        {
            //filtro proximas a salir
            var top = 6;
            var today = DateTime.Today;
            var upcomingReleases = await context.Movie
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            //filtro peliculas en cines
            var inTheathers = await context.Movie.
                Where(x => x.InTheaters)
                .Take(top)
                .ToListAsync();

            var result = new IndexMoviePageDTO();
            result.InTheaters = mapper.Map<List<MovieDTO>>(inTheathers);
            result.UpcomingReleases = mapper.Map<List<MovieDTO>>(upcomingReleases);

            return result;
        }

        //public async Task<ActionResult<List<MovieDTO>>> Get()
        //{
        //    var movies = await context.Movie.ToListAsync();
        //    if (movies == null) return NotFound();
        //    var moviesDto = mapper.Map<List<MovieDTO>>(movies);
        //    return Ok(moviesDto);
        //}


        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMovieDTO filterMovieDTO)
        {
            var moviesQueryable = context.Movie.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMovieDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMovieDTO.Title));
                //busca el titulo 
            }

            if (filterMovieDTO.InTheaters) //si le envia el InTheaters= True 
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }

            if (filterMovieDTO.UpcomingReleases) //si le envia el UpcomingReleases= True 
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMovieDTO.GenreId != 0)//si le envia el GenreId=numero de genre 
            {
                moviesQueryable = moviesQueryable.Where(x => x.MoviesGenres.Select(y => y.GenreId).Contains(filterMovieDTO.GenreId));
            }

            if (!string.IsNullOrWhiteSpace(filterMovieDTO.OrderingField))
            {
                //toca hacer validacion
                try
                {
                    //se require instalar paquete System.Linq.Dynamic.Core y poner using System.Linq.Dynamic.Core
                    moviesQueryable = moviesQueryable.OrderBy($"{filterMovieDTO.OrderingField} {(filterMovieDTO.AscendingOrder ? "ascending" : "descending")}");
                }
                catch (Exception)
                {

                    //logger.LogWarning($"Could not order by field {filterMovieDTO.OrderingField}");
                    return BadRequest($" el {filterMovieDTO.OrderingField} Campo no existe ");
                }
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable, filterMovieDTO.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMovieDTO.Pagination).ToListAsync();

            return mapper.Map<List<MovieDTO>>(movies);
        }

        /*sin moviesGenre y MoviesActors
         * // GET api/<MoviesController>/5
              [HttpGet("{id:int}", Name = "getMovie")]
              public async Task<ActionResult<MovieDTO>> Get(int id)
              {
                  var movie = await context.Movie.FirstOrDefaultAsync(x => x.Id == id);

                  if (movie == null) return NotFound();
                  var movieDto = mapper.Map<MovieDTO>(movie);
                  return movieDto;
              }*/


        // GET api/<MoviesController>/5
        [HttpGet("{id:int}", Name = "getMovie")]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            var movie = await context.Movie
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null) return NotFound();
            var movieDto = mapper.Map<MovieDetailsDTO>(movie);
            return movieDto;
        }

        // POST api/<MoviesController>
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<Movie>(movieCreationDTO); // se pasa a Tipo Movie

            if (movieCreationDTO.Poster != null) //guardar img
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movie.Poster =
                        await fileStorageService.SaveFile(content, extension, containerName, movieCreationDTO.Poster.ContentType);
                }
            }
            AnnotateActorsOrder(movie);
            context.Add(movie);
            await context.SaveChangesAsync();
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return CreatedAtRoute("getMovie", new { id = movie.Id }, movieDTO);
        }


        /// <summary>
        /// Ordena los actores para guardarlos en la Tabla MoviesActors
        /// </summary>
        /// <param name="movie"></param>
        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }

        // PUT api/<MoviesController>/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movieDb = await context.Movie.FirstOrDefaultAsync(m => m.Id == id);
            if (movieDb == null) return NotFound();
            var imgMovie = movieDb.Poster;

            movieDb = mapper.Map(movieCreationDTO, movieDb); // actualiza
            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movieDb.Poster =
                        await fileStorageService.EditFile(content, extension, containerName, movieDb.Poster,movieCreationDTO.Poster.ContentType);
                }
            }
            else
            {
                movieDb.Poster = imgMovie;
            }

            //borra los actores y generos de las tablas MoviesActors y MoviesGenders
            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movieDb.Id}; delete from MoviesGenres where MovieId = {movieDb.Id};");

            AnnotateActorsOrder(movieDb); //ordenar los nuevos datos de  MoviesActors y MoviesGenders
            await context.SaveChangesAsync();
            //return NoContent();
            var movieDTO = mapper.Map<MovieDTO>(movieDb);
            return CreatedAtRoute("getMovie", new { id = movieDb.Id }, movieDTO);

        }

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await context.Movie.AnyAsync(m => m.Id == id);
            if (exist)
            {
                var movie = await context.Movie.FirstOrDefaultAsync(x => x.Id == id);
                if (movie.Poster != null)
                {
                    await fileStorageService.DeleteFile(movie.Poster, containerName);
                }
                context.Remove(movie);
                await context.SaveChangesAsync();
                return NoContent();
            }

            return NotFound();
        }

    }
}