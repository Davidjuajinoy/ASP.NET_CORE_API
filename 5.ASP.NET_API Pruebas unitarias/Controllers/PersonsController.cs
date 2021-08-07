using ASP.NET_API.DTOs;
using ASP.NET_API.Entities;
using ASP.NET_API.Helper;
using ASP.NET_API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "Persons";

        public PersonsController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }




        [HttpGet(Name ="getPersons")]
        public async Task<List<PersonDTO>> Get()
        {
            var persons = await context.Persons.ToListAsync();
            return mapper.Map<List<PersonDTO>>(persons);
        }


        /*
         Require QueryableExtensions.cs, PaginationDTO, HttpContextExtensions 
        Solo funciona con Entity Framework y esta obsoleto porque requiere usar  UseRowNumberForPaging() que se usa en sqlserver 2008
        */
        //public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery] PaginationDTO pagination)
        //{
        //    var queryable = context.Persons.AsQueryable(); // datos para ordenar
        //    await HttpContext.InsertPaginationParametersInResponse(queryable,pagination.RecordsPerPage);

        //    //var persons = await context.Persons.ToListAsync();
        //    //llama el metodo de QueryableExtensions 
        //    var persons = await queryable.Paginate(pagination).ToListAsync();

        //    return mapper.Map<List<PersonDTO>>(persons);
        //}

        [HttpGet("{id:int}", Name = "GetPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {

            var person = await context.Persons.FirstOrDefaultAsync(p => p.Id == id);

            if (person != null)
            {
                var personDTO = mapper.Map<PersonDTO>(person);
                return personDTO;
            }
            return NotFound();

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PersonCreationDTO personCreationDTO)
        {
            var person = mapper.Map<Person>(personCreationDTO);

            context.Add(person);
            await context.SaveChangesAsync();

            var personDTO = mapper.Map<PersonDTO>(person);

            return CreatedAtRoute("GetPerson", new { id = person.Id }, personDTO);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("form")]
        public async Task<ActionResult> PostForm([FromForm] PersonCreationFormDTO personCreationFormDTO)
        {
            var person = mapper.Map<Person>(personCreationFormDTO);

            if (personCreationFormDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationFormDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(personCreationFormDTO.Picture.FileName);
                    person.Picture =
                        await fileStorageService.SaveFile(content, extension, containerName,
                                                            personCreationFormDTO.Picture.ContentType);
                }
            }

            context.Add(person);
            await context.SaveChangesAsync();
            var personDTO = mapper.Map<PersonDTO>(person);
            return CreatedAtRoute("GetPerson", new { id = personDTO.Id }, personDTO);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("{id:int}")]

        public async Task<ActionResult> Put(int id , [FromForm] PersonCreationFormDTO personCreationFormDTO)
        {
            var PersonDB = await context.Persons.FirstOrDefaultAsync(p => p.Id == id); //busca la persona

            if (PersonDB == null) return NotFound();

            var imgPerson = PersonDB.Picture;

            PersonDB = mapper.Map(personCreationFormDTO, PersonDB); //se actualiza mediante mapper y no ignora picture no se porque aunque se puso en AutoMapperProfile
            //da null la img si personCreationFormDTO es null ignorando la img de antes PersonDB, se corrige agregandole la img en otra variable

            if (personCreationFormDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationFormDTO.Picture.CopyToAsync(memoryStream); // lo copia en la variable memory stream
                    var content = memoryStream.ToArray(); //convierte la imagen en []bytes
                    var extension = Path.GetExtension(personCreationFormDTO.Picture.FileName); //coje desde el punto del nombre (img.png) = png
                    PersonDB.Picture = await fileStorageService.EditFile(content, extension, containerName, PersonDB.Picture ,personCreationFormDTO.Picture.ContentType); //se guarda la img en la carpeta etc
                }
            }
            else
            {
                PersonDB.Picture = imgPerson;
            }

            await context.SaveChangesAsync();
            //return Ok();

            var personDTO = mapper.Map<PersonDTO>(PersonDB);
            return CreatedAtRoute("GetPerson", new { id = personDTO.Id }, personDTO);


        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var exist = await context.Persons.AnyAsync(x => x.Id == Id);
            if (exist)
            {
                var person = await context.Persons.FirstOrDefaultAsync(x => x.Id == Id);
                if (person.Picture != null)
                {
                    await fileStorageService.DeleteFile(person.Picture, containerName);
                }
                //context.Remove(new Person { Id = Id }); //da error por utilizar la id arriba
                context.Remove(person);
                await context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

    }
}