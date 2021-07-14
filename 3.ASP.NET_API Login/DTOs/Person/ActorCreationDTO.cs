using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs.Person
{
    /// <summary>
    /// Clase objeto para llenar los datos en tabla MovieActors haciendo uso de mapper en MoviesController
    /// </summary>
    public class ActorCreationDTO
    {
        public int PersonId { get; set; }
        public string Character { get; set; }
    }
}
