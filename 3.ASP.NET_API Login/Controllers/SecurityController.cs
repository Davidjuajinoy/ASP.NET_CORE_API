using ASP.NET_API.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Controllers
{
    /// <summary>
    /// Se encuentra Securidad Hash y encryptacion
    /// Necesita el HashService
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly IDataProtector protectionProvider;
        private readonly HashService hashService;

        public SecurityController(IDataProtectionProvider protectionProvider, HashService hashService)
        {
            this.protectionProvider = protectionProvider.CreateProtector("PONERKEY");
            this.hashService = hashService;
            //Se recomienda en variables de entorno o en user secrets
        }

        [HttpGet]
        public IActionResult Get()
        {
            string plainText = "DavidJuajinoy";
            string encryptedText = protectionProvider.Protect(plainText);
            string decryptedText = protectionProvider.Unprotect(encryptedText);

            return Ok( 
                new
                {
                    plainText = plainText,
                    encryptedText = encryptedText,
                    decryptedText = decryptedText
                }
            );
        }



        /// <summary>
        /// No se desencrypta despues de cierto tiempo
        /// </summary>
        /// <returns>Un Error por encryted expired</returns>
        [HttpGet("TimeBound")]
        public async Task<ActionResult> TimeBound()
        {
            // protectorTimeBound se le pone limite al tiempo de cifrado
            var protectorTimeBound = protectionProvider.ToTimeLimitedDataProtector();

            string plainText = "DavidJuajinoy";
            string encryptedText = protectorTimeBound.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));
            await Task.Delay(6000);
            string decryptedText = protectorTimeBound.Unprotect(encryptedText);

            return Ok(
                new
                {
                    plainText = plainText,
                    encryptedText = encryptedText,
                    decryptedText = decryptedText
                }
            );
        }

        /// <summary>
        /// Funcion que se encarga del hash
        /// </summary>
        /// <returns>2 hash con diferentes salt</returns>
        [HttpGet("Hash")]
        public ActionResult GetHash()
        {
            string plainText = "DavidJuajinoy";
            var HashResult= hashService.Hash(plainText);
            var HashResult1 = hashService.Hash(plainText);
            return Ok(
               new
               {
                   plainText = plainText,
                   HashResult = HashResult,
                   HashResult1 = HashResult1
               }
           );
        }
    }
}
