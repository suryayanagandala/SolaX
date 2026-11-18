using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface ICartRepository
    {
        Task AddToCart(AddToCartDto dto);

        Task UpdateCart(UpdateCartItemDto dto);

        Task DeleteCartItem(int CartItemId);

        Task<CartResponseDto> GetCartResponse(string userId);

        Task<int> CountCartItems(string userId);
    }
}
