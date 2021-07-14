using ASP.NET_API.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController: ControllerBase
    {
        [HttpGet(Name ="getRoot")]
        public ActionResult<IEnumerable<Link>> Get()
        {
            List<Link> links = new List<Link>();


            links.Add(new Link(href: Url.Link("getRoot", new { }), rel: "self",method:"GET"));
            links.Add(new Link(href: Url.Link("createUser", new { }), rel: "create-user",method:"POST"));
            links.Add(new Link(href: Url.Link("login", new { }), rel:"login",method:"POST"));
            links.Add(new Link(href: Url.Link("getGenres", new { }), rel: "get-genres",method:"GET"));
            links.Add(new Link(href: Url.Link("getPersons", new { }), rel: "get-persons",method:"GET"));
            return links;
        }

    }
}
