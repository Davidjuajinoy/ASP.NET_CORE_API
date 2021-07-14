using ASP.NET_API.DTOs.Security;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Services
{
    public class HashService
    {
        /// <summary>
        /// Genera una salt random
        /// </summary>
        /// <param name="input">Texto que se quiere hashear</param>
        /// <returns>Retorna el input hasheado</returns>
        public HashResult Hash(string input)
        {
            //Generar Salt Random
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Hash(input, salt);
        }

        /// <summary>
        /// Es la funcion la cual se encarga de hacer el Hasheo
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        /// <returns>el Hash y el Salt</returns>
        public HashResult Hash(string input, byte[] salt)
        {
            //256 bits (10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: input,
                    salt : salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256/8
                ));

            return new HashResult()
            {
                Hash = hashed,
                Salt = salt
            };
        }
    }
}
