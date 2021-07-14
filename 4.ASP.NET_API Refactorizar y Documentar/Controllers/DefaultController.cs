using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Controllers
{
    /// <summary>
    /// Redireccionar localhost:44329 a Swagger por defecto
    /// </summary>
    [ApiExplorerSettings(IgnoreApi =true)]
    public class DefaultController : ControllerBase
    {
        [Route("/")]
        [Route("/swagger")]
        public RedirectResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
