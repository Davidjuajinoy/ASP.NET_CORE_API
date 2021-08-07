using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.testing
{
    public class TransferService
    {
        private readonly IValidateWireTranfer validateWireTranfer;

        public TransferService(IValidateWireTranfer validateWireTranfer)
        {
            this.validateWireTranfer = validateWireTranfer;
        }
        public void WireTransfer(Account origin, Account destionation, decimal amount)
        {
            var state = validateWireTranfer.validate(origin, destionation, amount);

            if (!state.IsSuccessful)
            {
                throw new ApplicationException(state.ErrorMessage);
            }

            origin.Funds -= amount;
            destionation.Funds += amount;
        }
    }
}
