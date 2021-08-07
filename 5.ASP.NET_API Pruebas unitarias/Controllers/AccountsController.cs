using ASP.NET_API.DTOs;
using ASP.NET_API.DTOs.Accounts;
using ASP.NET_API.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASP.NET_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;


        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }


        // POST api/<AccountsController>
        [HttpPost("Create", Name ="createUser" )]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            var user = new IdentityUser() { UserName = model.EmailAdress, Email = model.EmailAdress };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return await BuilToken(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        /// <summary>
        /// Renovar el token
        /// </summary>
        /// <returns></returns>
        [HttpGet("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> Renew()
        {
            var userInfo = new UserInfo()
            {
                EmailAdress = HttpContext.User.Identity.Name
            };

            return await BuilToken(userInfo);
        }

        /// <summary>
        /// Crear token JWT
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private async Task<UserToken> BuilToken(UserInfo userInfo)
        {
            //claims son identificadores en los cuales se puede confiar
            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userInfo.EmailAdress),
                    new Claim(ClaimTypes.Email, userInfo.EmailAdress),
                    new Claim("my valor que yo quiera", "Whatever valie I want")
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //tambien se puede asi
                };
            /*Para administrador*/
            var user = await userManager.FindByEmailAsync(userInfo.EmailAdress);
            var claimDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"])); //traer la key personal se convierte a bytes el string

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //credenciales  

            var expiration = DateTime.UtcNow.AddYears(1);

            JwtSecurityToken token = new JwtSecurityToken(

                issuer: null,//tu dominio , es la entidad que emite el token
                audience: null,//tu dominio, 
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );


            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };

        }

        // PUT api/<AccountsController>/5
        [HttpPost("Login",Name ="login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo model)
        {
            var result = await signInManager.PasswordSignInAsync(model.EmailAdress, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuilToken(model);
            }
            else
            {
                return BadRequest($"Invalid Login Atemp");
            }

        }

        /*Administrar roles*/

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("Users")]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            
            var users = await queryable.ToListAsync();
            return mapper.Map<List<UserDTO>>(users);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("Roles")]
        public async Task<ActionResult<List<string>>>GetRoles()
        {
            return await context.Roles.Select(x => x.Name).ToListAsync();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("AssignRole")]
        public async Task<ActionResult> AssignRole([FromBody] EditRoleDTO editRole)
        {
            //identificar el usuario
            var user = await userManager.FindByIdAsync(editRole.UserId);
            if (user == null)
            {
                return NotFound();
            }
            //asignacion del rol
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role,editRole.RoleName));
            //await userManager.AddToRoleAsync(user, editRole.RoleName); asi tambien se puede

            return NoContent();
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("RemoveRole")]
        public async Task<ActionResult> RemoveRole([FromBody] EditRoleDTO editRole)
        {
            //identificar el usuario
            var user = await userManager.FindByIdAsync(editRole.UserId);
            if (user == null)
            {
                return NotFound();
            }
            //asignacion del rol
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRole.RoleName));
            //await userManager.RemoveToRoleAsync(user, editRole.RoleName); asi tambien se puede

            return NoContent();
        }
    }
}
