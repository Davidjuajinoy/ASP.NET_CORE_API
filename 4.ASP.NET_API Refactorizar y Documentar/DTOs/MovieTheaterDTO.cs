using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs
{
    public class MovieTheaterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double DistanceMeters { get; set; }
        public double DistanceInKms { get { return DistanceMeters / 1000; } }
    }
}
