using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class CartRepository : ICartRepository
    {

        private readonly ApplicationDbContext _db;

        public CartRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddToCart(AddToCartDto dto)
        {
            try
            {
                var cart = await _db.Carts.Include(x => x.CartItems).FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.IsActive);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = dto.UserId,
                        IsActive = true,
                        CreatedAt = TimeUtility.NowIST()
                    };

                    await _db.Carts.AddAsync(cart);
                    await _db.SaveChangesAsync();
                }

                var productDetails = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && x.IsActive);
                if (productDetails == null)
                {
                    throw new Exception($"No ProductId found or inactive state");
                }

                var cartItems = cart.CartItems.FirstOrDefault(x => x.ProductId == dto.ProductId && x.IsActive);
                if (cartItems != null)
                {
                    cartItems.Quantity += dto.Quantity;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        Price = productDetails.Price,
                        IsActive = true
                    });

                    await _db.SaveChangesAsync();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteCartItem(int CartItemId)
        {
            try
            {
                var existId = await _db.CartItems.FindAsync(CartItemId);
                if (existId==null)
                {
                    throw new Exception($"No cartItem found {CartItemId}");
                }
                else
                {
                    existId.IsActive= false;
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateCart(UpdateCartItemDto dto)
        {
            var cartItem = await _db.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == dto.CartItemId);

            if (cartItem == null)
                throw new Exception("Cart item not found.");

            if (!cartItem.Cart.IsActive)
                throw new Exception("Cart is inactive.");

            cartItem.Quantity = dto.Quantity;
            cartItem.UpdatedAt = TimeUtility.NowIST();

            await _db.SaveChangesAsync();
        }


        public async Task<CartResponseDto> GetCartResponse(string userId)
        {
            try
            {
                
                var cart= await _db.Carts.Include(x=>x.CartItems.Where(c=>c.IsActive)).ThenInclude(x=>x.Product).ThenInclude(x=>x.ProductImages)
                    .FirstOrDefaultAsync(x=>x.UserId==userId&&x.IsActive);
                if (cart == null)
                {
                    throw new Exception("No Cart Item was found");
                }

                return new CartResponseDto
                {
                    CartId = cart.CartId,
                    Items = cart.CartItems.Select(x => new CartItemDto
                    {
                        CartItemId = x.CartItemId,
                        ProductId = x.ProductId,
                        ProductName = x.Product.ProductName,
                        Quantity = x.Quantity,
                        Price = x.Price,
                        ImageUrl = x.Product.ProductImages
                                       .Where(img => img.IsActive)
                                       .Select(img => img.ImageUrl)
                                       .FirstOrDefault()!,
                        Brand=x.Product.Brand,
                        Description=x.Product.Description,
                        Type=x.Product.Type,
                        Capacity=x.Product.Capacity
                    }).ToList(),

                    TotalPrice = cart.CartItems.Where(x => x.IsActive).Sum(x => x.Price * x.Quantity)

                };

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CountCartItems(string userId)
        {
            try
            {
                var countCartItems = await _db.Carts .Where(x => x.IsActive && x.UserId == userId).SelectMany(x => x.CartItems.Where(ci => ci.IsActive))
                                   .SumAsync(ci => ci.Quantity);   

                return countCartItems;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
