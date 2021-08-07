using ASP.NET_API.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_API.DTOs
{
    public class PersonCreationFormDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        [Required]
        public string Biography { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

        [FileSizeValidator(1)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }
    }
}