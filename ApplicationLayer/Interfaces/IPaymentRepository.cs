using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IPaymentRepository
    {
        Task<PaymentCreateDto> PaymentGateway(PaymentCreateDto dto);

        Task<QrResponseDto> GenerateQr(int orderId);

    }
}
