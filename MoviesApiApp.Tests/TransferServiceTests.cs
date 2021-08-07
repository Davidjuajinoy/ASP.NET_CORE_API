using ASP.NET_API.testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace MoviesApiApp.Tests
{
    [TestClass]
    public class TransferServiceTests
    {
        [TestMethod]
        public void wireTransferWithInsufficientFundsThrowsAnError()
        {

            //Preparation
            Account origin = new Account() { Funds = 0 };
            Account destination = new Account() { Funds = 0 };

            decimal amountToTranfer = 5;
            string errorMessage = "El origen de la cuenta no tiene dinero disponible";

            //Instalar moq
            var mockValidateWireTransfer = new Mock<IValidateWireTranfer>();

            mockValidateWireTransfer
                .Setup(x => x.validate(origin, destination, amountToTranfer))
                .Returns( new OperationResult(false,errorMessage) );


            var service = new TransferService(mockValidateWireTransfer.Object);
            Exception expetedException = null;
            //testing

            try
            {
               service.WireTransfer(origin, destination, amountToTranfer);

            }
            catch (Exception ex)
            {

                expetedException = ex;
            }

            //verification
            if (expetedException == null)
            {
                Assert.Fail("Se esperaba una excepcion");
            }

            Assert.IsTrue(expetedException is ApplicationException);
            Assert.AreEqual("El origen de la cuenta no tiene dinero disponible", expetedException.Message);

        }

        [TestMethod]
        public void WireTransferSuccess()
        {
            //Preparation
            Account origin = new Account() { Funds = 20 };
            Account destination = new Account() { Funds = 5 };

            decimal amountToTranfer = 5m;

            var mockValidateWireTransfer = new Mock<IValidateWireTranfer>();
                
            mockValidateWireTransfer
                .Setup(x => x.validate(origin,destination,amountToTranfer))
                .Returns(new OperationResult(true));

            
            var service = new TransferService(mockValidateWireTransfer.Object);

            //testing

            service.WireTransfer(origin, destination, amountToTranfer);
            //verification
            Assert.AreEqual(15, origin.Funds);
            Assert.AreEqual(10, destination.Funds);
        }
    }
}
