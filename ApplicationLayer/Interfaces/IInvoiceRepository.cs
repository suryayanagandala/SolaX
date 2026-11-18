using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<InvoiceDto> GetInvoice(int orderId);

        Task<byte[]> GeneratePdf(int orderId);

    }
}
