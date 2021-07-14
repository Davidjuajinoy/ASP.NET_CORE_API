using ASP.NET_API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieTheatersController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;



        public MovieTheatersController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterDTO>>> Get([FromQuery] FilterMovieTheatersDTO filterMovieTheatersDTO)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var usersLocation = geometryFactory.CreatePoint(new Coordinate(filterMovieTheatersDTO.Long, filterMovieTheatersDTO.Lat));

            var theaters = await context.movieTheaters
                .OrderBy(x => x.Location.Distance(usersLocation))
                .Where(x => x.Location.IsWithinDistance(usersLocation, filterMovieTheatersDTO.DistanceInKms * 1000))
                .Select(x => new MovieTheaterDTO { Id = x.Id, Name = x.Name, DistanceMeters = Math.Round(x.Location.Distance(usersLocation)) })
                .ToListAsync();
            return theaters;

        }
    }


}
