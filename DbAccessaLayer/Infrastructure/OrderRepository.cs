using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using MailKit.Search;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DbAccessaLayer.Infrastructure
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ISignalRRepository _signalRRepository;

        public OrderRepository(ApplicationDbContext db,ISignalRRepository signalRRepository)
        {
           _db=db;   
            _signalRRepository=signalRRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetOrderHistory(string userId)
        {
            try
            {
                
                var ordersList = await _db.Orders
                    .Where(x => x.UserId == userId && x.Payment.Any())
                    .Include(x => x.Delivery)        
                    .Include(x => x.OrderItem)
                        .ThenInclude(i => i.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(x => x.OrderItem)
                        .ThenInclude(i => i.Product)
                            .ThenInclude(p => p.Category)
                    .Include(x => x.Payment)
                    .AsNoTracking()
                    .ToListAsync();
                if (!ordersList.Any())
                    throw new Exception(MessageHelper.Get("Order", "OrderNotFound"));

                //  Order and project in memory
                var result = ordersList
                    .OrderByDescending(x => x.CreatedAt) 
                    .Select(x => new OrderDto
                    {
                        OrderId = x.OrderId,
                        UserId = x.UserId,
                        TotalAmount = x.TotalAmount,
                        Status = x.Status.ToString(),
                        DeliveryDate = x.Delivery?.DeliveryAt,
                        Items = x.OrderItem.Select(i => new OrderItemCreateDto
                        {
                            ProductId = i.ProductId,
                            ProductName = i.Product.ProductName,
                            Price = i.Product.Price,
                            Product = new ProductDto
                            {
                                Price = i.Product.Price,
                                Brand = i.Product.Brand,
                                Type = i.Product.Type,
                                Capacity = i.Product.Capacity,
                                CategoryName = i.Product.Category.CategoryName,
                                Description = i.Product.Description,
                                ImageUrl = i.Product.ProductImages.Select(img => img.ImageUrl).FirstOrDefault()
                            }
                        }).ToList(),
                        Payment = x.Payment.Select(p => new PaymentDto
                        {
                            PaymentMethod = p.PaymentMethod,
                            PaymentStatus = p.PaymentStatus,
                            PaymentDate = p.PaymentDate.DateTime
                        }).ToList()
                    })
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateOrderResponse> PlaceOrder(OrderCreateDto dto)
        {
            try
            {
                decimal totalAmount = 0;

                var orderItems = new List<OrderItems>();

                if (dto.IsBuyNow)
                {
                    if (dto.Quantity <= 0)
                        throw new Exception("Quantity must be greater than zero");
                    var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && x.IsActive);

                    if (product == null)
                    {
                        throw new Exception("No product found based on id or proudct is in active");
                    }

                    else
                    {
                        totalAmount = product.Price * dto.Quantity!.Value;
                        orderItems.Add(new OrderItems
                        {
                            ProductId = product.ProductId,
                            Quantity=dto.Quantity!.Value,
                            UnitPrice=product.Price,


                        });
                    }
                }

                else
                {
                    var cart = await _db.Carts.Include(x => x.CartItems).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.IsActive);

                    if (cart == null || !cart.CartItems.Any())
                    {
                        throw new Exception("Cart is empty");
                    }

                    totalAmount = cart.CartItems.Sum(x => x.Quantity * x.Product.Price);

                    foreach (var item in cart.CartItems)
                    {
                        orderItems.Add(new OrderItems
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.Product.Price
                        });
                    }

                    
                }


                var order = new Orders
                {
                    UserId = dto.UserId,
                    DeliveryAddress = dto.DeliveryAddress,
                    FullName = dto.DeliveryName,
                    PhoneNumber = dto.DeliveryPhone,
                    Status = OrderStatus.PAYMENT_PENDING,
                    TotalAmount = totalAmount,
                    IsActive = true,
                    CreatedAt=TimeUtility.NowIST()
                };

                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();

                foreach (var item in orderItems)
                {
                    item.OrderId = order.OrderId;
                    await _db.OrderItems.AddAsync(item);
                }

                await _db.SaveChangesAsync();

                var messages = new Notifications
                {
                    UserId = dto.UserId,
                    Type = NotificationType.Order,
                    Title = "Place Order",
                    Message = $"Your Order {order.OrderId} has status of {order.Status}"
                };
                await _db.Set<Notifications>().AddAsync(messages);
                await _db.SaveChangesAsync();

                var notificationDto = new NotificationDto
                {
                    UserId = dto.UserId,
                    NotificationId = messages.NotificationId,
                    NotificationType = messages.Type.ToString()!,
                    Title = messages.Title,
                    Message = messages.Message,
                    IsRead = false,
                    CreatedAt = messages.CreatedAt,

                };

                await this._signalRRepository.SendAsyncNotification(dto.UserId, notificationDto);

                return new CreateOrderResponse
                {
                    OrderId = order.OrderId,
                    OrderStatus=order.Status.ToString(),
                };

            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<OrderDto> GetOrderDetails(string userId, int orderId)
        {
            try
            {
                var details=await _db.Orders.Include(x=>x.OrderItem).ThenInclude(x=>x.Product).FirstOrDefaultAsync(x=>x.UserId == userId&&x.OrderId==orderId);
                if (details == null)
                {
                    throw new Exception($"Invalid UserId or Invalid OrderId");
                }
                else
                {
                    return  new OrderDto
                    {
                        OrderId = details.OrderId,
                        UserId=details.UserId,
                        Status = details.Status.ToString(),
                        TotalAmount = details.TotalAmount,
                        Items = details.OrderItem.Select(i => new OrderItemCreateDto
                        {
                            ProductId = i.ProductId,
                            ProductName = i.Product.ProductName,
                            Quantity = i.Quantity,
                            Price = i.UnitPrice
                        }).ToList()
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CancelOrder(int orderId, string userId)
        {
            var transcations = await _db.Database.BeginTransactionAsync();
            try
            {
                var order = await _db.Orders.Include(x => x.OrderItem).FirstOrDefaultAsync(x => x.OrderId == orderId && x.UserId == userId);
                if (order == null)
                {
                    throw new Exception(MessageHelper.Get("Order", "OrderNotFound"));
                }

                if (order.Status == OrderStatus.CONFIRMED)
                {
                    foreach (var item in order.OrderItem)
                    {
                        var product = await _db.Products.FindAsync(item.ProductId);
                        if (product == null)
                        {
                            throw new Exception(MessageHelper.Get("Products", "NotFound"));
                        }
                        product.Stock += item.Quantity;
                        _db.Products.Update(product);

                    }
                }
                order.Status = OrderStatus.CANCELLED;
                order.UpdatedAt = TimeUtility.NowIST();
                order.IsActive = false;
                _db.Orders.Update(order);

                var messages = new Notifications
                {
                    UserId = userId,
                    Type = NotificationType.Order,
                    Title = "Cancel Order",
                    Message = $"Your orderId {orderId} has cancelled"
                };

                await _db.Set<Notifications>().AddAsync(messages);

                var dto = new NotificationDto
                {
                    UserId = userId,
                    NotificationId = messages.NotificationId,
                    NotificationType = messages.Type.ToString()!,
                    Title = messages.Title,
                    Message = messages.Message,
                    IsRead = false,
                    CreatedAt = messages.CreatedAt,
                };

                await this._signalRRepository.SendAsyncNotification(userId, dto);

                await _db.SaveChangesAsync();
                await transcations.CommitAsync();
            }
            catch (Exception)
            {
                await transcations.RollbackAsync();
                throw;
            }
        }

        public async Task<UserCheckOutDto> GetUserDetails(string userId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.UserId == userId).Select(x => new UserCheckOutDto
                {
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber
                }).FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new Exception($"No UserId Found of {user}");
                }
                else
                {
                    return user;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<TackDelivery>> GetTrackDelivery(int orderId)
        {
            try
            {
                var delivery = await _db.Set<Delivery>().Include(x=>x.Order).Where(x=>x.Order.OrderId==orderId&&x.OrderId==orderId).FirstOrDefaultAsync();

                if(delivery == null)
                {
                    throw new Exception("No orderId found");
                }
                var list =new List<TackDelivery>();

                

                list.Add(new TackDelivery
                {
                    Stage = "Confirmed",
                    Status = "Completed",
                });

                list.Add(new TackDelivery
                {
                    Stage = "Shipped",
                    Status = delivery.Order.Status==OrderStatus.SHIPPED?"Active":"Pending",
                });
                list.Add(new TackDelivery
                {
                    Stage = "OutofDelivery",
                    Status = delivery.Status==DeliveryStatus.OutForDelivery.ToString()?"Active":"Pending",
                });
                list.Add(new TackDelivery
                {
                    Stage = "Delievered",
                    Status = delivery.Status==DeliveryStatus.Delivered.ToString()?"Active":"Pending",
                });






                return list;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
