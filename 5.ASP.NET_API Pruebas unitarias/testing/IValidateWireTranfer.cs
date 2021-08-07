using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.testing
{
    public interface IValidateWireTranfer
    {
        OperationResult validate(Account origin, Account destination, decimal amount);
    }
}
