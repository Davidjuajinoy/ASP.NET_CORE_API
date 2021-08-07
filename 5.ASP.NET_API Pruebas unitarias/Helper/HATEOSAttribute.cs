
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Helper
{
    /// <summary>
    /// Filtro que sirve para Validar que si puede ver el HATEOAS o no
    /// </summary>
    public class HATEOSAttribute: ResultFilterAttribute
    {
        /// <summary>
        /// Valida si existe el header con HATEOAS
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected bool ShouldIncludeHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;
            if (!IsSuccessfulResponse(result))
            {
                return false;
            }

            var header = context.HttpContext.Request.Headers["includeHATEOAS"];
            if (header.Count == 0)
            {
                return false;
            }

            var value = header[0];
            if (!value.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) 
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Valida si la peticion es diferente a un 200.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool IsSuccessfulResponse(ObjectResult result)
        {
            if (result == null || result.Value == null)
            {
                return false;
            }
            if (result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }

            return true;

        }
    }
}
