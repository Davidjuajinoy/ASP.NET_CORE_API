using ASP.NET_API.DTOs;
using ASP.NET_API.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Helper
{
    /// <summary>
    /// Filtro para genre
    /// </summary>
    public class GenreHATEOASAttribute : HATEOSAttribute
    {
        private readonly LinksGenerator linksGenerator;

        public GenreHATEOASAttribute(LinksGenerator linksGenerator)
        {
            this.linksGenerator = linksGenerator;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var includeHATEOAS = ShouldIncludeHATEOAS(context);

            if (!includeHATEOAS)
            {
                await next();
                return;
            }

            await linksGenerator.Generate<GenreDTO>(context, next);
        }
    }
}
