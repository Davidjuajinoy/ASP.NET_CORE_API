using ASP.NET_API.DTOs.Person;
using ASP.NET_API.Helper;
using ASP.NET_API.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs
{
    public class MovieCreationDTO
    {
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        [FileSizeValidator(1)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Poster { get; set; }

        //se llama al ModelBinder para agrege los parametros a la lista
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GendersId { get; set; }
        //se llama al ModelBinder para agrege los parametros a la lista
        [ModelBinder(BinderType =typeof(TypeBinder<List<ActorCreationDTO>>))]
        public List<ActorCreationDTO> Actors { get; set; }
    }
}
