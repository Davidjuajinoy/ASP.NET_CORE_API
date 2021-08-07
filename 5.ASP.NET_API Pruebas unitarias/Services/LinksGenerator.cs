using ASP.NET_API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Services
{

    /// <summary>
    /// Generar el objeto helper ulr que nos ayuda a que los enlaces apunten a nuestro api web
    ///Se debe insertar los servicios requeridos IActionContextAccessor, GenreHATEOASAttribute, LinksGenerator
    /// </summary>
    public class LinksGenerator
    {
        private readonly IUrlHelperFactory urlHelperFactory;
        private readonly IActionContextAccessor actionContextAccessor;

        public LinksGenerator(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            this.urlHelperFactory = urlHelperFactory;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper GetUrlHelper()
        {
            return urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public async Task Generate<T>(ResultExecutingContext context, ResultExecutionDelegate next) where T: class , IGenerateHATEOASLinks, new ()
        {
            var urlHelper = GetUrlHelper();
            var result = context.Result as ObjectResult;

            var model = result.Value as T;

            if (model == null)
            {
                var modelList = result.Value as List<T> ??
                    throw new ArgumentNullException($"Se espera una instancia de tipo {typeof(T)}");
                modelList.ForEach(dto => dto.GenerateLinks(urlHelper));
                var individual = new T();
                result.Value = individual.GenerateLinksCollection(modelList, urlHelper);
                await next();
            }
            else
            { 
                model.GenerateLinks(urlHelper);
                await next();
            }
        }
    }
}
