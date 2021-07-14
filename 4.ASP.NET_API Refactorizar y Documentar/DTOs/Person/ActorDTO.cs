using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs.Person
{
    /// <summary>
    /// Se usa en MoviesDetailsDTO 
    /// </summary>
    public class ActorDTO
    {
        public int PersonId { get; set; }
        public string Character { get; set; }
        public string PersonName { get; set; }
    }
}
