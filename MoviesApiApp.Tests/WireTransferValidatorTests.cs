using ASP.NET_API.testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApiApp.Tests
{
    [TestClass]
    public class WireTransferValidatorTests
    {
        [TestMethod]
        public void ValidateReturnErrorWhenInsufficientsFunds()
        {
            Account origin = new Account() { Funds = 0 };
            Account destination = new Account() { Funds = 0 };

            decimal amountToTranfer = 5;
            var service = new WireTransferValidator();

            var result = service.validate(origin, destination, amountToTranfer);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("El origen de la cuenta no tiene dinero disponible", result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateReturnSuccessfulOperation()
        {
            Account origin = new Account() { Funds = 7 };
            Account destination = new Account() { Funds = 0 };

            decimal amountToTranfer = 5m;
            var service = new WireTransferValidator();

            var result = service.validate(origin, destination, amountToTranfer);
            Assert.IsTrue(result.IsSuccessful);
        }
    }
}
