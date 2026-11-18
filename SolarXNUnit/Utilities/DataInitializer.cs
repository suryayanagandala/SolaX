using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SolarXNUnit.Utilities
{
    public class DataInitializer
    {
        public static IQueryable<Users> UsersData()
        {
            var users = new List<Users>
            {
                new Users{
                    UserId="ERCXET",
                    FullName="Surya",
                    EmailId="surya@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Surya@2003"),
                    PhoneNumber="987654321",
                    RoleId=3,
                    IsActive=true,
                    Address="vijaywada kondapalli",
                    CreatedBy="Admin",
                    CreatedDate=new DateTimeOffset(2025,01,01,10,00,00,TimeSpan.Zero)
                },

                 new Users{
                UserId="ABX123",
                FullName="Priya Sharma",
                EmailId="priya.sharma@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Priya@123"),
                PhoneNumber="9123456780",
                RoleId=2,
                IsActive=true,
                Address="Hyderabad Banjara Hills",
                CreatedBy="Admin",
                CreatedDate=new DateTimeOffset(2025,01,01,10,00,00,TimeSpan.Zero)
                  },

                    new Users{
                        UserId="LMN456",
                        FullName="Rahul Verma",
                        EmailId="rahul.verma@example.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Rahul@2025"),
                        PhoneNumber="9988776655",
                        RoleId=1,
                        IsActive=true,
                        Address="Bengaluru Whitefield",
                        CreatedBy="Admin",
                        CreatedDate=new DateTimeOffset(2025,01,01,10,00,00,TimeSpan.Zero)
                    },

            new Users{
                UserId="XYZ789",
                FullName="Anita Reddy",
                EmailId="anita.reddy@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Anita@2026"),
                PhoneNumber="9876501234",
                RoleId=3,
                IsActive=false,
                Address="Chennai Anna Nagar",
                CreatedBy="Admin",
                CreatedDate=new DateTimeOffset(2025,01,01,10,00,00,TimeSpan.Zero)
              }
            };

            return users.AsQueryable();
        }

        public static IQueryable<OtpManager> OtpManagers()
        {
            var data = new List<OtpManager>
            {
                new OtpManager{Id=1,Email="surya@gmail.com",Otptext="452478",Otptype="Forgot Password",
                CreatedDate=TimeUtility.NowIST(),IsUsed=false,Expiration=TimeUtility.NowIST().AddMinutes(5),token=Guid.Parse("EA007833-178D-4C11-8A78-C4BA6D897F12")}
            };
            return data.AsQueryable();
        }

        public static List<Users> GetUsers()
        {
            return new List<Users>
            {
                new Users
                {
                    UserId = "ERCXET",
                    FullName = "Surya",
                    EmailId = "surya@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Surya@2003"),
                    PhoneNumber = "9876543210",
                    RoleId = 3,
                    IsActive = true,
                    Address = "Vijayawada Kondapalli",
                    CreatedBy = "Admin",
                    CreatedDate = DateTimeOffset.UtcNow
                },
                new Users
                {
                    UserId = "ABX123",
                    FullName = "Priya Sharma",
                    EmailId = "priya.sharma@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Priya@123"),
                    PhoneNumber = "9123456780",
                    RoleId = 2,
                    IsActive = true,
                    Address = "Hyderabad Banjara Hills",
                    CreatedBy = "Admin",
                    CreatedDate = DateTimeOffset.UtcNow
                },
                  new Users{
                        UserId="LMN456",
                        FullName="Rahul Verma",
                        EmailId="rahul.verma@example.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Rahul@2025"),
                        PhoneNumber="9988776655",
                        RoleId=1,
                        IsActive=true,
                        Address="Bengaluru Whitefield",
                        CreatedBy="Admin",
                        CreatedDate=new DateTimeOffset(2025,01,01,10,00,00,TimeSpan.Zero)
                    },

            new Users{
                UserId="XYZ789",
                FullName="Anita Reddy",
                EmailId="anita.reddy@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Anita@2026"),
                PhoneNumber="9876501234",
                RoleId=3,
                IsActive=false,
                Address="Chennai Anna Nagar",
                CreatedBy="Admin",
                CreatedDate=new DateTimeOffset(2025,01,01,10,00,00,TimeSpan.Zero)
              }
            };
        }

   
        // CATEGORY
        

        public static List<Category> GetCategories()
        {
            return new List<Category>
            {
               new Category
               {
                CategoryId = 1,
                CategoryName = "Solar",
                Slug = "solar-panels",
                CreatedAt=TimeUtility.NowIST(),
                IsActive = true,
               },
               new Category
               {
                   CategoryId = 2,
                   ParentCategoryId = 1,
                   CategoryName=GenerateSlug("solar child-1"),
                   Slug="solar-child-1",
                   CreatedAt=TimeUtility.NowIST(),
                   IsActive=true,
               },
               new Category
               {
                CategoryId = 3,
                CategoryName = "Solar",
                Slug = "solar-panels",
                CreatedAt=TimeUtility.NowIST(),
                IsActive=false
               },
                new Category
               {
                CategoryId = 4,
                CategoryName = "SolarX",
                 Slug =GenerateSlug("SolarX"),
                CreatedAt=TimeUtility.NowIST(),
                IsActive=true
               }
            };
        }

        
        // PRODUCTS
      
        public static List<Products> GetProducts()
        {
            return new List<Products>
            {
                new Products
                {
                    ProductId = 1,
                    ProductName = "Solar Panel 500W",
                    CategoryId = 1, 
                    Brand = "Tata",
                    Type = "Panel",
                    CapacityValue = 500,
                    Capacity = "500W",
                    Price = 15000,
                    Warranty_Years = 5,
                    Stock = 10,
                    Description = "High efficiency solar panel",
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsActive = true,
                    ProductImages=new List<ProductImage>
                    {
                        new ProductImage
                        {
                            ImageId = 1,
                            ProductId=1,
                            ImageUrl="uploads/products/imge.png",
                            CreatedAt=TimeUtility.NowIST(),
                            IsActive=true,
                            
                        }
                    }
                },

                 new Products
                 {
                    ProductId = 2,
                    ProductName = "Solar Panel 300W",
                    CategoryId = 2,
                    Brand = "Tata",
                    Type = "Panel",
                    CapacityValue = 500,
                    Capacity = "500W",
                    Price = 15000,
                    Warranty_Years = 5,
                    Stock = 10,
                    Description = "High efficiency solar panel",
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsActive = true,
                    ProductImages=new List<ProductImage>
                    {
                        new ProductImage
                        {
                            ImageId = 2,
                            ProductId=2,
                            ImageUrl="uploads/products/imge.png",
                            CreatedAt=TimeUtility.NowIST()
                        }
                    }

                 },

                  new Products
                  {
                    ProductId = 3,
                    ProductName = "Solar Panel 100W",
                    CategoryId = 4,
                    Brand = "Tata",
                    Type = "Panel",
                    CapacityValue = 500,
                    Capacity = "500W",
                    Price = 15000,
                    Warranty_Years = 5,
                    Stock = 10,
                    Description = "High efficiency solar panel",
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsActive = true,
                    ProductImages=new List<ProductImage>
                    {
                        new ProductImage
                        {
                            ImageId = 3,
                            ProductId=2,
                            ImageUrl="uploads/products/imge.png",
                            CreatedAt=TimeUtility.NowIST()
                        }
                    },



                  },
                    new Products
                    {
                    ProductId = 4,
                    ProductName = "Solar Panel 800W",
                    CategoryId = 4,
                    Brand = "Tata",
                    Type = "Panel",
                    CapacityValue = 500,
                    Capacity = "500W",
                    Price = 15000,
                    Warranty_Years = 5,
                    Stock = 10,
                    Description = "High efficiency solar panel",
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsActive = false,
                    ProductImages=new List<ProductImage>
                    {
                        new ProductImage
                        {
                            ImageId = 4,
                            ProductId=4,
                            ImageUrl="uploads/products/imge.png",
                            CreatedAt=TimeUtility.NowIST()
                        }
                    }
                    }



            };
        }

   
        // ORDERS
      

        public static List<Orders> GetOrders()
        {
            return new List<Orders>
            {
                new Orders
                {
                    OrderId = 1,
                    UserId = "ERCXET", 
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    Status = OrderStatus.CREATED,
                    TotalAmount = 15000,
                    PhoneNumber = "9876543210",
                    FullName = "Surya",
                    IsActive = true,
                    DeliveryAddress = "Vijayawada Kondapalli",
                    CreatedAt=TimeUtility.NowIST(),
                    OrderItem = new List<OrderItems>
                    {
                        new OrderItems
                        {
                            OrderItemId = 1,
                            ProductId = 1,  
                            Quantity = 1,
                            UnitPrice = 15000
                        }
                    }
                },

                new Orders
                {
                    OrderId = 2,
                    UserId = "ABX123",
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    Status = OrderStatus.CREATED,
                    TotalAmount = 15000,
                    PhoneNumber = "9876543210",
                    FullName = "Surya",
                    IsActive = true,
                    DeliveryAddress = "Vijayawada Kondapalli",
                    CreatedAt=TimeUtility.NowIST(),
                    OrderItem = new List<OrderItems>
                    {
                        new OrderItems
                        {
                            OrderItemId = 2,
                            ProductId = 1,
                            Quantity = 1,
                            UnitPrice = 15000
                        }
                    }
                },
                new Orders
                {
                    OrderId = 3,
                    UserId="XYZ789",
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    Status = OrderStatus.CREATED,
                    TotalAmount = 15000,
                    PhoneNumber = "9876543210",
                    FullName = "Surya",
                    IsActive = true,
                    DeliveryAddress = "Vijayawada Kondapalli",
                    CreatedAt=TimeUtility.NowIST(),
                    OrderItem = new List<OrderItems>
                    {
                        new OrderItems
                        {
                            OrderItemId = 3,
                            ProductId = 1,
                            Quantity = 1,
                            UnitPrice = 15000
                        }
                    }
                },
                new Orders
                {
                    OrderId = 4,
                    UserId="LMN456",
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    Status = OrderStatus.DELIVERED,
                    TotalAmount = 15000,
                    PhoneNumber = "9876543210",
                    FullName = "Surya",
                    IsActive = true,
                    DeliveryAddress = "Vijayawada Kondapalli",
                    CreatedAt=TimeUtility.NowIST(),
                    OrderItem = new List<OrderItems>
                    {
                        new OrderItems
                        {
                            OrderItemId = 4,
                            ProductId = 3,
                            Quantity = 1,
                            UnitPrice = 15000
                        }
                    }
                }
            };
        }

        
        // PAYMENTS
    

        public static List<Payments> GetPayments()
        {
            return new List<Payments>(); 
        }

       
        // CART
        
        public static List<Cart> GetCarts()
        {
             return new List<Cart>
             {
                new Cart
                {
                    CartId = 1,
                    UserId = "ABX123",
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsActive = true,
                    CartItems = new List<CartItem>
                    {
                        new CartItem
                        {
                            CartItemId = 1,
                            ProductId = 1,
                            Quantity = 2,
                            Price = 15000
                        }
                    }
                }
             };
        }

      
        // TECHNICIANS
        public static List<Technicians> GetTechnicians()
        {
            return new List<Technicians>
            {
                new Technicians
                {
                    TechnicianId = 1,
                    Specialization = "Installer",
                    IsAvailable = true,
                    IsActiveDeliveries = 0,
                    UserId = "ABX123",
                    Qualification = "Diploma",
                    Salary=30000,
                    ExperienceYears=1,
                    Service_Price=750
                },
                new Technicians
                {
                    UserId="XYZ789",
                    TechnicianId = 2,
                    Specialization =Specialization.Maintenance.ToString(),
                    IsAvailable = true,
                    IsActiveDeliveries = 0,
                    Qualification = "Diploma"
                }
            };
        }


        public static List<Roles> GetRoles()
        {
            return new List<Roles>
            {
                new Roles { RoleId = 2, RoleName = "User" },
                new Roles { RoleId = 3, RoleName = "Admin" }
            };
        }


        public static List<Delivery> GetDeliveries()
        {
            return new List<Delivery>
            {
                new Delivery {OrderId=1,DeliveryId=1,AssignedToTechId=1,AssignedAt=TimeUtility.NowIST(),
                DeliveryAt=TimeUtility.NowIST().AddDays(7),AssignedBy="Admin",Status=DeliveryStatus.OutForDelivery.ToString() },

                 new Delivery {OrderId=2,DeliveryId=2,AssignedToTechId=1,AssignedAt=TimeUtility.NowIST(),
                DeliveryAt=TimeUtility.NowIST().AddDays(7),AssignedBy="Admin",Status=DeliveryStatus.Delivered.ToString() },

                 new Delivery {OrderId=2,DeliveryId=3,AssignedToTechId=null,AssignedAt=TimeUtility.NowIST(),
                DeliveryAt=TimeUtility.NowIST().AddDays(7),AssignedBy="Admin",Status=DeliveryStatus.Pending.ToString() },

                new Delivery {OrderId=3,DeliveryId=9,AssignedToTechId=1,AssignedAt=TimeUtility.NowIST(),
                DeliveryAt=TimeUtility.NowIST().AddDays(7),AssignedBy="Admin",Status=DeliveryStatus.Delivered.ToString() },
                 new Delivery {OrderId=4,DeliveryId=10,AssignedToTechId=1,AssignedAt=TimeUtility.NowIST(),
                DeliveryAt=TimeUtility.NowIST().AddDays(7),AssignedBy="Admin",Status=DeliveryStatus.Delivered.ToString() }
            };
        }


        public static List<Installations> GetInstallations()
        {
            return new List<Installations>
            {
                new Installations
                {
                    InstallationId="INST-20260201-0001",
                    DeliveryId=1,
                    IsActive=true,
                    OrderId=1,
                    CreatedAt=TimeUtility.NowIST().AddDays(-5),
                    Status=InstallationStatus.Completed.ToString(),
                    CompletedDate=TimeUtility.NowIST(),
                    IsPanelCreated=false
                },
                 new Installations
                {
                    InstallationId="INST-20260201-0002",
                    DeliveryId=2,
                    IsActive=true,
                    OrderId=2,
                    CreatedAt=TimeUtility.NowIST().AddDays(-5),
                    Status=InstallationStatus.Completed.ToString(),
                    CompletedDate=TimeUtility.NowIST(),
                    IsPanelCreated = false
                },

                  new Installations
                  {
                    InstallationId="INST-20260201-0003",
                    DeliveryId=9,
                    IsActive=false,
                    OrderId=2,
                    CreatedAt=TimeUtility.NowIST().AddDays(-5),
                    Status=InstallationStatus.Completed.ToString(),
                    CompletedDate=TimeUtility.NowIST().AddDays(-1),
                    IsPanelCreated = true
                },
                  new Installations
                  {
                    InstallationId="INST-20260201-0004",
                    DeliveryId=10,
                    IsActive=true,
                    OrderId=4,
                    CreatedAt=TimeUtility.NowIST().AddDays(-5),
                    Status=InstallationStatus.Completed.ToString(),
                    CompletedDate=TimeUtility.NowIST().AddDays(-1),
                    IsPanelCreated = true
                },


            };

        }

        public static List<InstallationReports> GetInstallationReports()
        {
            return new List<InstallationReports>
            {
                new InstallationReports
                {
                    InstallationId="INST-20260201-0001",
                    Notes="Initial inspection completed. All equipment delivered and ready for installation.",
                    Image_Url="https://example.com/images/ins00001_1.jpg",
                    CheckList="something is checked",
                    CreatedAt=TimeUtility.NowIST()

                }
            };
        }

        public static List<InstallationTechnician> GetInstallationTechnicians()
        {
            return new List<InstallationTechnician>
            {
                new InstallationTechnician
                {
                    InstallationId="INST-20260201-0001",
                    TechnicianId=1,
                    ServicePrice=250,

                }
            };
        }


        public static List<Notifications> GetNotifications()
        {
            return new List<Notifications>
            {
                new Notifications
                {
                     UserId = "ABX123",
                    Message = "test db purpose",
                    Type = NotificationType.System,
                    IsActive = true,
                    CreatedAt = TimeUtility.NowIST(),
                    
                    IsRead=false,
                    Title="db test",

                },
                new Notifications
                {
                    UserId = "ABX123",
                    Message = "test db purpose",
                    Type = NotificationType.System,
                    IsActive = true,
                    CreatedAt = TimeUtility.NowIST(),
                    
                    IsRead=false,
                    Title="db test",
                }
            };
        }

        public static List<Invoices> GetInvoices()
        {
            return new List<Invoices>
            {
                new Invoices
                {
                    OrderId=1,
                    CustomerName="Verma",
                    CustomerEmail="verma@gmail.com",
                    PaymentMethod="ONLINE",
                    Amount=1000,
                    Tax=0,
                    TotalAmount=1000,
                    InvoiceDate=TimeUtility.NowIST(),
                    PaymentStatus="Success",
                    PhoneNumber="9009900990",
                    OrderStatus=OrderStatus.CONFIRMED.ToString(),
                }
            };
        }

        public static List<Payments> GetListPayments()
        {
            return new List<Payments>
            {
               new Payments
               {
                   PaymentId=1000,
                   OrderId=101,
                   PaymentMethod="UPI",
                   TransactionId="TXN24C2D6824F18",
                   PaymentStatus="SUCCESS",
                   PaymentDate=TimeUtility.NowIST(),
               }
            };
        }

        public static List<MaintenanceReports> GetMaintenanceReports()
        {
            return new List<MaintenanceReports>
            {
                new MaintenanceReports
                {
                    ReportId=1000,
                    RequestId="R3934582",
                    ReplacedParts="panel",
                    CreatedAt=TimeUtility.NowIST()
                }
            };
        }

        public static List<MaintenanceRequests> GetMaintenanceRequests() {
            return new List<MaintenanceRequests>
            {
                new MaintenanceRequests
                {
                    RequestId="R3934581",
                    InstallationId="INST-20260201-0001",
                    UserId="ABX123",
                    Status=MaintenanceStatus.Assigned.ToString(),
                    CreatedAt=TimeUtility.NowIST()
                },
                new MaintenanceRequests
                {
                    RequestId="R3934582",
                    InstallationId="INST-20260201-0001",
                    UserId="ABX123",
                    Status=MaintenanceStatus.Completed.ToString()   ,
                    CreatedAt=TimeUtility.NowIST()
                },
                new MaintenanceRequests
                {
                    UserId="LMN456",
                    RequestId="R3934583",
                    InstallationId="INST-20260201-0004",
                    Status =MaintenanceStatus.Pending.ToString() ,
                    CreatedAt=TimeUtility.NowIST()
                }
            }; 
        }

        public static List<MaintenanceTechnicians> GetMaintenanceTechnicians()
        {
            return new List<MaintenanceTechnicians>
            {
                new MaintenanceTechnicians
                {
                    RequestId="R3934581",
                    TechnicianId=2,
                    ServicePrice=750
                },
                new MaintenanceTechnicians
                {
                    RequestId="R3934582",
                    TechnicianId=2,
                    ServicePrice=750,
                }
            };
        }

        private static string GenerateSlug(string name)
        {
            return Regex.Replace(name.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');
        }

    }



}

