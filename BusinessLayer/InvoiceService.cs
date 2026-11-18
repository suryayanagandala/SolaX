using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class InvoiceService
    {
        private readonly IUnitOfWork unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<InvoiceDto> GetInvoiceData(int orderId) => await this.unitOfWork.invoiceRepository.GetInvoice(orderId);

        public async Task<byte[]>GeneratePdf(int orderId)
        {
           return await this.unitOfWork.invoiceRepository.GeneratePdf(orderId);
        }
    }
}
