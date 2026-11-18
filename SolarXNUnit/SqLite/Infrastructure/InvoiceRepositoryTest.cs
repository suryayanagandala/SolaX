using CoreModels.CustomMessages;
using DbAccessaLayer.Infrastructure;
using iText.Pdfua.Checkers.Utils.Ua1;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrstructure
{
    public class InvoiceRepositoryTest:SqliteTestBase
    {
        private InvoiceRepository invoiceRepository;
        [SetUp]
        public void SetUp()
        {
            invoiceRepository = new InvoiceRepository(_db);

        }

        [Test]
        public void GetrInvoice_Failure_OrderId()
        {
            try
            {
                int orderId = 90;
                Assert.ThrowsAsync<NotFoundException>(async () => await invoiceRepository.GetInvoice(orderId));
            }
            catch (Exception ex)
            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(errorMessages);
            }
        }

        [Test]
        public async Task GetInvoice_Success()
        {
            var result = await invoiceRepository.GetInvoice(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Surya", result.CustomerName);
            Assert.AreEqual(0, result.Tax);
        }

        //generatePdf
        [Test]
        public async Task GeneratePdf_Success()
        {
            var result = await invoiceRepository.GeneratePdf(1);

            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }
    }
}
