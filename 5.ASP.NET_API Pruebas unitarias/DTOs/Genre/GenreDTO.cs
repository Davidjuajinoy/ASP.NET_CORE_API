using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_API.DTOs
{
    public class GenreDTO : IGenerateHATEOASLinks
    {
        public int Id { get; set; }
        [Required]
        [StringLength(60)]
        public string Name { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();

        public void GenerateLinks(IUrlHelper urlHelper)
        {
            Links.Add(new Link(urlHelper.Link("GetGenre", new { id = Id }), "get-genre", method: "GET"));
            Links.Add(new Link(urlHelper.Link("putGenre", new { Id = Id }), "put-genre", method: "PUT"));
            Links.Add(new Link(urlHelper.Link("deleteGenre", new { Id = Id }), "delete-genre", method: "DELETE"));
        }

        public ResourceCollection<GenreDTO> GenerateLinksCollection<GenreDTO>(List<GenreDTO> dtos, IUrlHelper urlHelper)
        {
            var resourceCollection = new ResourceCollection<GenreDTO>(dtos); // se crea una nueva lista
            resourceCollection.Links.Add(new Link(urlHelper.Link("getGenres", new { }), "get-genres", method: "GET"));
            resourceCollection.Links.Add(new Link(urlHelper.Link("createGenre", new { }), "create-genre", method: "DELETE"));
            return resourceCollection;
        }
    }
}