using System.ComponentModel.DataAnnotations;

namespace ASP.NET_API.DTOs
{
    public class GenreDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(60)]
        public string Name { get; set; }
    }
}