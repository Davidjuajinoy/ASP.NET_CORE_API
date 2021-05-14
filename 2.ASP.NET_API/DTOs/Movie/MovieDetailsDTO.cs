using ASP.NET_API.DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs.Movie
{
    /// <summary>
    /// Se usa en el controlador MovieController y en el metodo get(id)
    /// </summary>
    public class MovieDetailsDTO : MovieDTO
    {
        public List<GenreDTO> Genders { get; set; }
        public List<ActorDTO> Actors { get; set; }
    }
}
