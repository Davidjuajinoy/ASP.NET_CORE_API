using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Entities
{
    public class MovieTheater
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public Point Location { get; set; } //se debe instalar EF nettopologysuite
    }
}
