using ASP.NET_API.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        //[FirstLetterUpperCase] //validacion personalizada
        public string Name { get; set; }
    }
}
