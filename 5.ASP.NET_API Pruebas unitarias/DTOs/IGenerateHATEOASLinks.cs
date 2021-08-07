using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs
{
    /// <summary>
    /// Interfaz con metodos para general los links
    /// </summary>
    public interface IGenerateHATEOASLinks
    {
        void GenerateLinks(IUrlHelper urlHelper);
        ResourceCollection<T> GenerateLinksCollection<T>(List<T> dtos, IUrlHelper urlHelper);
    }
}
