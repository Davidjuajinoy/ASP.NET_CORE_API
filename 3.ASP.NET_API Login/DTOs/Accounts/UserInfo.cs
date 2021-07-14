using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs.Accounts
{
    public class UserInfo
    {
        [EmailAddress]
        [Required]
        public string EmailAdress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
