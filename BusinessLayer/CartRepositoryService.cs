using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class CartRepositoryService
    {
        private readonly IUnitOfWork unitOfWork;

        public CartRepositoryService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;   
        }

        public async Task AddToCartAsync(AddToCartDto dto)
        {
            await this.unitOfWork.cartRepository.AddToCart(dto);
        }

        public async Task UpdateCartAsync(UpdateCartItemDto dto)
        {
            await this.unitOfWork.cartRepository.UpdateCart(dto);
        }

        public async Task RemoveCartItem(int id)
        {
            await this.unitOfWork.cartRepository.DeleteCartItem(id);
        }

        public async Task<CartResponseDto> GetUserCart(string userId)
        {
            return await  this.unitOfWork.cartRepository.GetCartResponse(userId);
        }

        public async Task<int> GetCountOfCartItems(string userId)
        {
            return await this.unitOfWork.cartRepository.CountCartItems(userId);
        }
    }
}
