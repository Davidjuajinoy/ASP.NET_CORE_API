using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.testing
{
    public class WireTransferValidator : IValidateWireTranfer
    {
        public OperationResult validate(Account origin, Account destination, decimal amount)
        {
            if (amount > origin.Funds)
            {
                return new OperationResult(false, "El origen de la cuenta no tiene dinero disponible");
            }

            //OTHER VALIDATIONS

            return new OperationResult(true);
        }
    }
}
