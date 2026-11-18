using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentCreateDto> PaymentGateway(PaymentCreateDto dto)
        {
           return await _unitOfWork.paymentRepository.PaymentGateway(dto);
        }

        public async Task<QrResponseDto> GenerateQr(int orderId)
        {
            return await _unitOfWork.paymentRepository.GenerateQr(orderId);
        }
    }
}
