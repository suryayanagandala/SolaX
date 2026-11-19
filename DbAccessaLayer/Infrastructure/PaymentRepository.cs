using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AssignmentSettings _assignmentSettings;
        private readonly ISignalRRepository signalRRepository;
        public PaymentRepository(ApplicationDbContext db,IOptions<AssignmentSettings> options,ISignalRRepository signalRRepository)
        {
            _db = db;
            _assignmentSettings = options.Value;
            this.signalRRepository = signalRRepository;
        }

        public async Task<PaymentCreateDto> PaymentGateway(PaymentCreateDto dto)
        {
            using var transactions = await _db.Database.BeginTransactionAsync();
            try
            {

                var order = await _db.Orders.Include(x => x.OrderItem).ThenInclude(x => x.Product).FirstOrDefaultAsync(
                    x => x.OrderId == dto.OrderId && x.IsActive);

                if (order == null)
                {
                    throw new Exception("Order not found ot inactive");
                }

                decimal totalAmount = order.OrderItem.Sum(x => x.Quantity * x.Product.Price);

                if (await _db.Payments.AnyAsync(x => x.OrderId == dto.OrderId))
                    throw new Exception("Already payment exist");

                var payment = new Payments
                {
                    OrderId = dto.OrderId,
                    PaymentMethod = dto.PaymentMethod,
                    PaymentStatus = dto.PaymentMethod,
                    TransactionId = GenerateTransactionId(),
                    Amount = totalAmount,
                    PaymentDate = TimeUtility.NowIST()
                };

                if (dto.PaymentMethod == "COD")
                {
                    payment.PaymentStatus = "PENDING";
                    
                }
                else
                {
                    payment.PaymentStatus = "SUCCESS";
                   
                }
                Console.WriteLine($"orderId:{order.UserId}");
                await ConfirmOrder(dto.OrderId, order.UserId);

                await _db.Payments.AddAsync(payment);

                _db.Orders.Update(order);
              
                await _db.SaveChangesAsync();
             
                await AssignDeliveriesToTechnician(dto.OrderId);
                await CreateInvoice(order.OrderId);
                var notification = new Notifications
                {
                    UserId = order.UserId,
                    Type = NotificationType.Order,
                    Title = "Order confirmed",
                    IsActive = false,
                    Message = $"your order {payment.OrderId} is now {OrderStatus.CONFIRMED.ToString()}",
                    CreatedAt = TimeUtility.NowIST()
                };

                await _db.Set<Notifications>().AddAsync(notification);
                await _db.SaveChangesAsync();
                Console.WriteLine($" Notification saved with ID {notification.NotificationId}");

                var signal = new NotificationDto
                {
                    UserId = notification.UserId,
                    NotificationId = notification.NotificationId,
                    NotificationType = notification.Type.ToString()!,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt,
                };

                await signalRRepository.SendAsyncNotification(notification.UserId, signal);



                await transactions.CommitAsync();
                return new PaymentCreateDto
                {
                    OrderId=payment.OrderId,
                    PaymentMethod=payment.PaymentMethod,
                    UserId=order.UserId,
                    
                };


            }
            catch (Exception)
            {
                await transactions.RollbackAsync();
                throw;
            }
        }

        private async Task CreateInvoice(int orderId)
        {
            try
            {
                var order=await _db.Orders.Include(x=>x.Payment).Include(x=>x.User).FirstOrDefaultAsync(x=>x.OrderId==orderId);
                if (order == null)
                {
                    throw new Exception("no order found");
                }
                var payment = order.Payment.OrderByDescending(x => x.PaymentDate).FirstOrDefault()
                    ?? await _db.Payments.Where(x => x.OrderId == orderId).OrderByDescending(x => x.PaymentDate).FirstOrDefaultAsync();
                var invoice = new Invoices
                {
                    OrderId = orderId,
                    CustomerName = order.FullName,
                    CustomerEmail = order.User.EmailId,
                    PhoneNumber = order.PhoneNumber,
                    Amount = order.TotalAmount,
                    PaymentMethod = order.Payment.FirstOrDefault()?.PaymentMethod ?? "Cash",
                    TotalAmount = payment?.Amount ?? order.TotalAmount,
                    OrderStatus = order.Status.ToString(),
                    PaymentStatus = order.Payment.FirstOrDefault()?.PaymentStatus ?? "Pending",
                    InvoiceDate = TimeUtility.NowIST()
                };

                await _db.Invoices.AddAsync(invoice);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QrResponseDto> GenerateQr(int orderId)
        {

            var order=await _db.Orders.FirstOrDefaultAsync(x=>x.OrderId== orderId);   
            if (order == null)
            {
                throw new Exception("OrderId was not found");
            }
            //dummy text encoded into qr
            string dummyText = $"ORDER-{orderId}-PAYMENT";

            //using var ensures it will be disposed automatically after use.
            using var qrGenerator = new QRCoder.QRCodeGenerator();

            //qrData holds the raw QR matrix.
            using var qrData = qrGenerator.CreateQrCode(dummyText, QRCoder.QRCodeGenerator.ECCLevel.Q);

            //Wraps the QR data into a QRCode object.

            using var qrCode = new QRCoder.QRCode(qrData);

            //get image pixels with 20
            using var bitmap = qrCode.GetGraphic(20);

            //Creates a memory stream to hold the image data in memory instead of saving to disk.
            using var ms = new MemoryStream();


            //now the images is stored in png format with binary format in memory
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            // Converts the PNG image bytes into a Base64 string
            var base64 = Convert.ToBase64String(ms.ToArray());

            var qrResponse = new QrResponseDto 
            { QrCodeImage = $"data:image/png;base64,{base64}",
                ExpiresIn = 300 
            };

            return (qrResponse);



        }

        private  string GenerateTransactionId()
        {
            return "TXN" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
        } 

        //private async  Task ReduceStock(Orders orders)
        //{
        //    try
        //    {
        //        foreach(var item in orders.OrderItem)
        //        {
        //            var product = item.Product;
        //            if (product.Stock < item.Quantity)
        //                throw new Exception($"Out of stock: {product.ProductName}");

        //            product.Stock -= item.Quantity;
        //            _db.Products.Update(product);
                    
        //        }
        //        await _db.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        //private async Task ClearUserCart(string userId)
        //{
        //    var cart = await _db.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        //    if (cart != null)
        //    {
        //        cart.CartItems.Clear();
        //    }
        //}

        //private  void RestoreStock(Orders order)
        //{
        //    foreach ( var item in order.OrderItem)
        //    {
        //        item.Product.Stock += item.Quantity;
        //    }
        //}

        private async Task ConfirmOrder(int orderId, string userId)
        {

            try
            {
                var order = await _db.Orders.Include(x => x.OrderItem).FirstOrDefaultAsync(x => x.OrderId == orderId && x.UserId == userId);
                if (order == null)
                {
                    throw new Exception("No order is found please check again");
                }

                var payment = order.Payment.FirstOrDefault(x=>x.OrderId==order.OrderId);

                //if (order.Payment.Any(x=>x.OrderId==order.OrderId))
                //{
                //    throw new Exception("Already payment exist");
                //}

                foreach (var item in order.OrderItem)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new Exception("No product found");
                    }

                    product.Stock -= item.Quantity;
                    _db.Products.Update(product);


                }

                order.Status = OrderStatus.CONFIRMED;
                _db.Orders.Update(order);

                var userCarts = await _db.Carts.Where(x => x.UserId == userId).ToListAsync();
                foreach (var cart in userCarts) cart.IsActive = false;
                await _db.SaveChangesAsync();


            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AssignDeliveriesToTechnician(int orderId)
        {
            try
            {
                var order = await _db.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);

                if (order == null)
                {
                    throw new Exception("No Order found");
                }
                var technician = await _db.Technicians.Where(x => x.Specialization == "Installer" && x.IsAvailable==true)
                    .OrderBy(x=>x.IsActiveDeliveries).ThenBy(x=>x.TechnicianId).FirstOrDefaultAsync();

                Console.WriteLine(technician == null? "NO TECHNICIAN FOUND": $"TECH FOUND: {technician.TechnicianId}");

                var delivery = new Delivery
                {
                    OrderId = orderId,
                    AssignedToTechId = technician?.TechnicianId,
                    DeliveryAt = DateTimeOffset.UtcNow.AddDays(_assignmentSettings.AssignedDelivery),
                    Status = technician == null ? "Pending" : "Assigned",
                    AssignedAt = TimeUtility.NowIST(),
                    AssignedBy="Admin",
                };
                

                    await _db.Deliveries.AddAsync(delivery);

                    if (technician!=null)
                    {
                        technician.IsActiveDeliveries += 1;
                        if (technician.IsActiveDeliveries >= 5)
                        {
                            technician.IsAvailable = false;
                        }

                        var notification = new Notifications
                        {
                            UserId = technician.UserId,
                            Type = NotificationType.Delivery,
                            Title = "Delivery Assigned",
                            IsActive = false,
                            Message = $"you have assigned new  delivery",
                            CreatedAt=TimeUtility.NowIST(),
                        };

                        await _db.Set<Notifications>().AddAsync(notification);
                        await _db.SaveChangesAsync();
                    

                        var signal = new NotificationDto
                        {
                            UserId = notification.UserId,
                            NotificationId = notification.NotificationId,
                            NotificationType = notification.Type.ToString()!,
                            Title = notification.Title,
                            Message = notification.Message,
                            IsRead = false,
                            CreatedAt = notification.CreatedAt,
                        };

                       await signalRRepository.SendAsyncNotification(notification.UserId, signal);
                    }

                    await _db.SaveChangesAsync();
                
            }
            catch (Exception)
            {
                throw;
            }
        }

      
        

        
    }
}
