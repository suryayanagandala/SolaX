using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using SolarXNUnit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.SeedData
{
    public static class TestDataSeeder
    {
        public static void SeedBasicOrder(ApplicationDbContext context)
        {
            try
            {
                var roles = DataInitializer.GetRoles();
                context.Roles.AddRange(roles);
                context.SaveChanges();

                var users = DataInitializer.GetUsers();
                context.Users.AddRange(users);
                context.SaveChanges();

                // ---------- CATEGORIES ----------
                var categories = DataInitializer.GetCategories();
                context.Categories.AddRange(categories);
                context.SaveChanges();

                // ---------- PRODUCTS ----------
                var products = DataInitializer.GetProducts();

                
                context.Products.AddRange(products);
                context.SaveChanges();



                // ---------- ORDERS------
                var orders = DataInitializer.GetOrders();

                foreach (var order in orders)
                {
                    context.Orders.Add(order);
                }

             
                context.SaveChanges();

                // ---------- CART--------
                var carts = DataInitializer.GetCarts();

                foreach (var cart in carts)
                {
                    context.Carts.Add(cart);
                }

               
                context.SaveChanges();

                // TECHNICIANS
                var technicians = DataInitializer.GetTechnicians();

                foreach (var tech in technicians)
                {
                    context.Technicians.Add(tech);
                }

                

                var payment = new Payments
                {
                    PaymentId = 1000,
                    OrderId = orders[0].OrderId,
                    Order = orders[0],
                    PaymentMethod = "COD",
                    PaymentStatus = "Success",
                    TransactionId = "TXN12345",
                    Amount = orders[0].TotalAmount,
                    PaymentDate = DateTimeOffset.UtcNow, 
                    IsActive = true
                };
                context.Payments.Add(payment);
                context.SaveChanges();

                var delivery = DataInitializer.GetDeliveries();
                foreach (var d in delivery)
                {
                    context.Deliveries.Add(d);
                }


                context.SaveChanges();


                var installations= DataInitializer.GetInstallations();
                foreach(var installation in installations)
                {
                    context.Installations.Add(installation);
                }

                context.SaveChanges();


                var notifications= DataInitializer.GetNotifications();
                foreach (var notification in notifications)
                {
                    context.Notifications.Add(notification);
                }
                context.SaveChanges();

                var invoices = DataInitializer.GetInvoices();
                foreach(var invoice in invoices){
                    context.Invoices.Add(invoice);
                }
                context.SaveChanges();

                var installationTechnicians= DataInitializer.GetInstallationTechnicians();
                foreach(var item in installationTechnicians)
                {
                    context.InstallationTechnicians.Add(item);
                }
                context.SaveChanges();


                //-----Installation Reports--------//

                var reports=DataInitializer.GetInstallationReports();
                foreach (var item in reports)
                {
                    context.InstallationReports.Add(item);
                }

                context.SaveChanges();


                // Maintenance Reports

                var maintenances=DataInitializer.GetMaintenanceReports();
                foreach(var item in maintenances)
                {
                    context.MaintenanceReports.Add(item);
                }
                context.SaveChanges();

                //Maintenabce Technciians
                var maintenanceTechs = DataInitializer.GetMaintenanceTechnicians();
                foreach (var tech in maintenanceTechs)
                {
                    context.MaintenanceTechnicians.Add(tech);
                }
                context.SaveChanges();

                // Maintenance request

                var requests=DataInitializer.GetMaintenanceRequests();
                foreach (var request in requests)
                {
                    context.MaintenanceRequests.Add(request);
                }
                context.SaveChanges();

                context.AddRange(
                    new Panel
                    {
                        PanelId = 1,
                        UserId = "ERCXET",
                        PanelName = "Solar Panles",
                        Capacity = "600W"
                    },
                    new Panel
                    {
                        PanelId = 2,
                        PanelName="Bi panles",
                        UserId= "LMN456",
                        Capacity="500W"
                    });

                context.SaveChanges();

                context.SolarPerformances.AddRange(
                    new SolarPerformance
                    {
                        UserId= "ERCXET",
                        PanelId = 1,
                        Power = 500,
                        Generated_at = TimeUtility.NowIST().AddMonths(-1), 
                        IsActive = true
                    },
                    new SolarPerformance
                    {
                        UserId= "LMN456",
                        PanelId = 2,
                        Power = 500,
                        Generated_at = TimeUtility.NowIST(), 
                        IsActive = true
                    });

                 context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MAIN ERROR:");
                Console.WriteLine(ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("INNER ERROR:");
                    Console.WriteLine(ex.InnerException.Message);
                }

                throw;
            }
        }
    }


    public class FakeSignalRRepository : ISignalRRepository
    {
        public bool WasCalled { get; private set; }

        public Task SendAlertsMessages(PanelAnomalyDetail panelAnomalyDetail)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }

        public Task SendAsyncNotification(string userId, NotificationDto dto)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }

        public Task SendNotificationAdmin(string userId, NotificationDto notificationDto)
        {
            throw new NotImplementedException();
        }

        public Task SendNotificationForSolarPanles(string userId, SolarPerformanceDto data)
        {
            throw new NotImplementedException();
        }
    }
}
