using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.testing
{
    public class OperationResult
    {
        public OperationResult(bool isSuccessful, string errorMessage = null)
        {
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
    }
}
