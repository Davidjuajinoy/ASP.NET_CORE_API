using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs.Movie
{
    /// <summary>
    /// Para mostrar los filtros al inicio en movies
    /// </summary>
    public class IndexMoviePageDTO
    {
        public List<MovieDTO> UpcomingReleases{ get; set; }
        public List<MovieDTO> InTheaters{ get; set; }
    }
}
