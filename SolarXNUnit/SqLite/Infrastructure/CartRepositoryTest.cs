using CoreModels.DTO_S;
using DbAccessaLayer.Infrastructure;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrstructure
{
    public class CartRepositoryTest:SqliteTestBase
    {
        private CartRepository cartRepository;

        [SetUp]
        public void SetUp()
        {
            cartRepository=new CartRepository(_db);
        }

        [Test]
        public async Task AddToCart_Should_CreateCart_And_AddItem()
        {
            var dto = new AddToCartDto
            {
                UserId = "ERCXET",
                ProductId = 1,
                Quantity = 2,

            };

            await cartRepository.AddToCart(dto);
            var cart = await _db.Carts.Include(x => x.CartItems).FirstOrDefaultAsync(x => x.UserId == "ERCXET");

            Assert.IsNotNull(cart);
            Assert.AreEqual(1, cart.CartItems.Count);
            

        }

        [Test]
        public async Task AddToCart_Should_IncreaseQuantity_When_ItemExists()
        {
            var dto = new AddToCartDto
            {
                UserId = "ABX123",
                ProductId = 1,
                Quantity = 2,

            };
            await cartRepository.AddToCart(dto);
            var updatedCart = await _db.Carts.Include(x => x.CartItems).FirstAsync();

            Assert.AreEqual(4, updatedCart.CartItems.First().Quantity);
        }


        [Test]
        public void AddToCart_Should_ThrowException_When_Product_NotFound()
        {
            var dto = new AddToCartDto
            {
                UserId = "user1",
                ProductId = 999,
                Quantity = 1
            };

            Assert.ThrowsAsync<Exception>(async () =>await cartRepository.AddToCart(dto));
        }

        //DeleteCartItem
        [Test]
        public void DeleteCartItem_Failure()
        {
            int cartItemId = 89;
            Assert.ThrowsAsync<Exception>(async()=>await  cartRepository.DeleteCartItem(cartItemId));
        }

        [Test]
        public async Task DeleteCartItem_Success()
        {
            int cartItemId = 1;
            await cartRepository.DeleteCartItem(cartItemId);
            _db.SaveChanges();

            var updatedCart=_db.CartItems.FirstOrDefault(x=>x.CartItemId==1);
            if(updatedCart != null)
            {
                Assert.That(updatedCart.IsActive, Is.EqualTo(false));
            }
        }

        //UpdateCart
        [Test]
        public void UpdateCart_Failure_Invalid_CartItemId()
        {
            var dto = new UpdateCartItemDto
            {
                CartItemId = 12,
                Quantity=1
            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await cartRepository.UpdateCart(dto));
            Assert.That(ex.Message, Does.Contain("Cart item not found"));
        }


        [Test]
        public async Task UpdateCartItem_Success()
        {
            var dto = new UpdateCartItemDto
            {
                CartItemId = 1,
                Quantity = 1
            };

            await cartRepository.UpdateCart(dto);
            _db.SaveChanges();

            var updatedCart=_db.CartItems.First(x=>x.CartItemId==1);
            Assert.That(updatedCart.Quantity, Is.EqualTo(1));
        }

        //CartResponse
        [Test]
        public void GetCartReponse_Faliure_FakeId()
        {
            string userId = "FakeId";
            var ex = Assert.ThrowsAsync<Exception>(async () => await cartRepository.GetCartResponse(userId));
            Assert.That(ex.Message, Is.EqualTo("No Cart Item was found"));
        }

        [Test]
        public async Task GetCartResponse_Success()
        {
            string userId = "ABX123";
            var result=await cartRepository.GetCartResponse(userId);
            Assert.IsNotNull(result);
        }

        //CountCartItems
        [Test]
        public async Task  CountCartItems_Success()
        {
            string userId = "ABX123";

            var result = await cartRepository.CountCartItems(userId);
            Assert.IsNotNull(result);
            
        }
    }


}
