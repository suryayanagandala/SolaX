using CoreModels.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
//using static iText.IO.Util.IntHashtable;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.Contracts;
namespace CoreModels
{
    public class ApplicationDbContext:DbContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options,IHttpContextAccessor httpContextAccessor):base(options)
        {
            this.httpContextAccessor = httpContextAccessor; 
        }

        public virtual DbSet<Roles> Roles { get; set; }

        public virtual DbSet<Users>Users { get; set; }

        public virtual DbSet<Technicians> Technicians{get;set;}
        public virtual DbSet<Products> Products { get; set; }

        public virtual DbSet<ProductImage> ProductImages { get; set; }

        public virtual DbSet<Cart> Carts { get; set; }

        public virtual DbSet<CartItem> CartItems { get; set; }

        public virtual DbSet<Orders> Orders { get; set; }

        public virtual DbSet<OrderItems> OrderItems { get; set; }

        public virtual DbSet<Payments> Payments { get; set; }

        public virtual DbSet<Delivery> Deliveries { get; set; }

        public virtual DbSet<Installations> Installations { get; set; }

        public virtual DbSet<InstallationTechnician> InstallationTechnicians { get; set; }

        public virtual DbSet<InstallationReports> InstallationReports { get; set; }

        public virtual DbSet<MaintenanceRequests> MaintenanceRequests { get; set; }

        public virtual DbSet<MaintenanceTechnicians> MaintenanceTechnicians { get; set; }

        public virtual DbSet<MaintenanceReports> MaintenanceReports { get; set; }

        public virtual DbSet<Notifications> Notifications { get; set; }

        public virtual DbSet<SolarPerformance> SolarPerformances { get; set; }

        public virtual DbSet<Invoices> Invoices { get; set; }

        public virtual DbSet<OtpManager> OtpManagers { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<ProductReview> ProductReviews { get; set; }

        public virtual DbSet<Panel> Panels { get; set; }

        public virtual DbSet<AuditLog> AuditLog { get; set; }

        public virtual DbSet<AccessoryCategoryMapping> AccessoryCategoryMappings { get; set; }

        public virtual DbSet<AccessPermissions> AccessPermissions { get; set; }

        public bool DisableAudit { get; set; } = false;


        public virtual DbSet<RefreshTokens> RefreshTokens {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>().HasKey(x => x.UserId);

            modelBuilder.Entity<Users>().HasOne(u => u.Role).WithMany(u => u.Users).HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Technicians>().HasOne(t => t.User).WithOne(u => u.Technician).HasForeignKey<Technicians>(t => t.UserId).HasPrincipalKey<Users>(u => u.UserId); ;

            modelBuilder.Entity<ProductImage>().HasOne(x => x.Product).WithMany(x => x.ProductImages).HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<Cart>().HasOne(x => x.Users).WithMany(x => x.Cart).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<CartItem>().HasKey(ci => ci.CartItemId);

            modelBuilder.Entity<CartItem>().HasOne(x => x.Cart).WithMany(x => x.CartItems).HasForeignKey(x => x.CartId);

            modelBuilder.Entity<CartItem>().HasOne(x => x.Product).WithMany(c => c.CartItem).HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<Orders>().HasOne(x => x.User).WithMany(x => x.Order).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<OrderItems>().HasOne(x => x.Order).WithMany(x => x.OrderItem).HasForeignKey(x => x.OrderId);

            modelBuilder.Entity<OrderItems>().HasOne(x => x.Product).WithMany(c => c.OrderItem).HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<Payments>().HasOne(x => x.Order).WithMany(c => c.Payment).HasForeignKey(x => x.OrderId);

            modelBuilder.Entity<Delivery>().HasOne(x => x.Order).WithOne(c => c.Delivery).HasForeignKey<Delivery>(x => x.OrderId);

            modelBuilder.Entity<Delivery>().HasOne(x => x.AssignedToTech).WithMany(c => c.Deliveries).HasForeignKey(x => x.AssignedToTechId);

            modelBuilder.Entity<Installations>().HasOne(x => x.Order).WithMany(c => c.Installation).HasForeignKey(x => x.OrderId);

            modelBuilder.Entity<Installations>().HasOne(x => x.delivery).WithMany(x => x.Installations).HasForeignKey(x => x.DeliveryId);

            modelBuilder.Entity<InstallationTechnician>().HasOne(x => x.Technician).WithMany(c => c.Installations).HasForeignKey(x => x.TechnicianId);

            modelBuilder.Entity<InstallationReports>().HasOne(x => x.Installation).WithMany(c => c.Reports).HasForeignKey(x => x.InstallationId);

            modelBuilder.Entity<MaintenanceRequests>().HasOne(x => x.User).WithMany(c => c.MaintenanceRequest).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<MaintenanceRequests>().HasOne(x => x.Installation).WithMany(c => c.MaintenanceRequest).HasForeignKey(x => x.InstallationId);

            modelBuilder.Entity<MaintenanceTechnicians>().HasOne(x => x.Technician).WithMany(c => c.Maintenance).HasForeignKey(x => x.TechnicianId);

            modelBuilder.Entity<MaintenanceReports>().HasOne(x => x.Request).WithMany(c => c.MaintenanceReports).HasForeignKey(x => x.RequestId);

            modelBuilder.Entity<Notifications>().HasOne(x => x.User).WithMany(c => c.Notifications).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<SolarPerformance>().HasOne(x => x.User).WithMany(c => c.solarPerformances).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Invoices>().HasOne(x => x.Order).WithMany(c => c.Invoices).HasForeignKey(x => x.OrderId);

            modelBuilder.Entity<Notifications>().Property(n => n.Type).HasConversion<string>().HasMaxLength(50);

            modelBuilder.Entity<Technicians>().Property(x=>x.Qualification).HasConversion<string>().HasMaxLength(50);

            modelBuilder.Entity<Technicians>().Property(x=>x.Specialization).HasConversion<string>().HasMaxLength(50);

            modelBuilder.Entity<Technicians>().Property(x=>x.Descipline).HasConversion<string>().HasMaxLength(50);

            modelBuilder.Entity<Orders>().Property(x => x.Status).HasConversion<string>().HasMaxLength(100);

            modelBuilder.Entity<Category>().HasOne(x => x.ParentCategory).WithMany(x => x.SubCategories).HasForeignKey(x => x.ParentCategoryId);

            //modelBuilder.Entity<Products>().HasOne(c=>c.Category).WithMany(x=>x.Products).HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<ProductReview>().HasOne(c=>c.Product).WithMany(x=>x.ProductReviews).HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<ProductReview>().HasOne(c => c.Users).WithMany(x => x.ProductReviews).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Payments>().Property(x => x.PaymentDate).ValueGeneratedOnAdd();

            modelBuilder.Entity<ProductImage>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<Products>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<ProductReview>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<Orders>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<Notifications>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<Invoices>().Property(x =>x.InvoiceDate).ValueGeneratedOnAdd();

            modelBuilder.Entity<Delivery>().Property(x => x.AssignedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<Category>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<InstallationReports>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<Installations>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<MaintenanceReports>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<MaintenanceRequests>().Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            modelBuilder.Entity<Payments>().HasIndex(p => p.OrderId).IsUnique();

            modelBuilder.Entity<Delivery>().HasIndex(x => x.OrderId).IsUnique();

            modelBuilder.Entity<AccessoryCategoryMapping>().HasOne(m => m.SourceCategory).WithMany().HasForeignKey(m => m.SourceCategoryId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccessoryCategoryMapping>().HasOne(m => m.AccessoryCategory).WithMany().HasForeignKey(m => m.AccessoryCategoryId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Products>().ToTable(tb => tb.HasTrigger("Tr_Sp_Unit"));

            modelBuilder.Entity<Category>().ToTable(x => x.HasTrigger("Tr_AutoMappingAccesories"));


            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                var dateTimeOffsetConverter =
                    new ValueConverter<DateTimeOffset, DateTime>(
                        v => v.UtcDateTime,
                        v => new DateTimeOffset(v, TimeSpan.Zero));

                var nullableDateTimeOffsetConverter =
                    new ValueConverter<DateTimeOffset?, DateTime?>(
                        v => v.HasValue ? v.Value.UtcDateTime : null,
                        v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entity.GetProperties())
                    {
                        if (property.ClrType == typeof(DateTimeOffset))
                            property.SetValueConverter(dateTimeOffsetConverter);

                        if (property.ClrType == typeof(DateTimeOffset?))
                            property.SetValueConverter(nullableDateTimeOffsetConverter);
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (DisableAudit)
                return await base.SaveChangesAsync(cancellationToken);
            ChangeTracker.DetectChanges();

            var auditEntries = new List<AuditLog>();

            var userId = httpContextAccessor?.HttpContext?.User?.FindFirst("UserId")?.Value ?? "System";
            var userRole = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog ||
                    entry.State == EntityState.Detached ||
                    entry.State == EntityState.Unchanged)
                    continue;

                var audit = new AuditLog
                {
                    TableName = entry.Metadata.GetTableName(),
                    Action = entry.State.ToString(),
                    UserId = userId,
                    UserRole = userRole,
                    ChangedAt = DateTime.UtcNow
                };

                var oldValues = new Dictionary<string, object>();
                var newValues = new Dictionary<string, object>();

                foreach (var property in entry.Properties)
                {
                    if (property.Metadata.IsPrimaryKey())
                        continue;

                    string propertyName = property.Metadata.Name;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            newValues[propertyName] = property.CurrentValue ?? string.Empty;
                            break;

                        case EntityState.Deleted:
                            oldValues[propertyName] = property.OriginalValue ?? string.Empty;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                oldValues[propertyName] = property.OriginalValue ?? string.Empty;
                                newValues[propertyName] = property.CurrentValue ?? string.Empty;
                            }
                            break;
                    }
                }

                audit.OldValues = oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues);
                audit.NewValues = newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues);

                auditEntries.Add(audit);
            }

            // Add audit logs BEFORE saving so everything is in one transaction
            if (auditEntries.Any())
                AuditLog.AddRange(auditEntries);

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
       



    
}
