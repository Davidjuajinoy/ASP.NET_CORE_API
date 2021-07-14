using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Helper
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName; //buscamos el nombre de la propiedad
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName); //buscar el valor de propiedad
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<T>(valueProviderResult.FirstValue);
                //var deserializedValue = JsonConvert.DeserializeObject<List<int>>(valueProviderResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue); //asigna el valor a la propiedad
            }
            catch
            {

                bindingContext.ModelState.TryAddModelError(modelName,"Value is invalid for type List<int>");
            }
            return Task.CompletedTask;
        }
    }
}
