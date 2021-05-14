using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Validations
{
    public class ContentTypeValidator : ValidationAttribute
    {
        private readonly string[] validContentTypes; //por si el usuario quiere agregarlos manualmente
        private readonly string[] imageContentTypes = new string[] { "image/jpeg", "image/png","image/gif" }; //por defecto

        //si el usuario lo agrega manualmente se ejecuta este
        public ContentTypeValidator(string[] validContentTypes)
        {
            this.validContentTypes = validContentTypes;
        }

        //si el usuario no agrega nada se ponen los img por defecto
        public ContentTypeValidator(ContentTypeGroup contentTypeGroup)
        {
            switch (contentTypeGroup)
            {
                //se sobreescribe el array de string vacio y se llena con los datos por defecto
                case ContentTypeGroup.Image:
                    validContentTypes = imageContentTypes;
                    break;
                default:
                    break;
            }
        }
       
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var fileForm = value as IFormFile;

            if (fileForm == null )
            {
                return ValidationResult.Success;
            }

            if (!validContentTypes.Contains(fileForm.ContentType))
            {
                return new ValidationResult($"Content type should be one of the following: {string.Join(", ", validContentTypes)}");
            }

            return ValidationResult.Success;
        }
    }

    public enum ContentTypeGroup
    {
        Image
    }
}
