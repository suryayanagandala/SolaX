using ApplicationLayer.Interfaces;
using CoreModels;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRoleRepository RoleRepository { get; }

        public IUserRepository UserRepository {  get; }

        public ITechnicianRepository TechnicianRepository { get; }

        public IAdminRepository AdminRepository { get; }

        public IProductsRepository ProductsRepository { get; }

     

        public ICategoryRepository category { get; }

        public IProductReviewRepository productReviewRepository { get; }

        public ICartRepository cartRepository { get; }

        public IOrderRepository orderRepository {  get; }

        public IHomeRepository homeRepository { get; }

        public IPaymentRepository paymentRepository { get; }

        public IDeliveryInterface deliveryInterface { get; }

        public IInvoiceRepository invoiceRepository { get; }

        public IInstallRepository installRepository { get; }

        public INoticationRepositry noticationRepositry { get; }

        public IMaintenanceRepository maintenanceRepository {  get; }

        //public ISignalRRepository signalRRepository { get; }

        public ISolarPerformaceRepository solarPerformaceRepository { get; }

        public IRefreshTokenRepository refreshTokenRepository {  get; }

        public IAccessRepository accessRepository { get; }

        public IAccessRepository2 accessRepository2 {  get; }

        private IDbContextTransaction? _transaction;

        public ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db,IRoleRepository roleRepository, IUserRepository userRepository, 
            ITechnicianRepository technicianRepository, IAdminRepository adminRepository, IProductsRepository productsRepository,
           ICategoryRepository category,IProductReviewRepository productReviewRepository,
            ICartRepository cartRepository,IOrderRepository orderRepository,IHomeRepository homeRepository,IPaymentRepository paymentRepository,
            IDeliveryInterface deliveryInterface, IInvoiceRepository invoiceRepository, IInstallRepository installRepository,INoticationRepositry noticationRepositry,
            IMaintenanceRepository maintenanceRepository, ISolarPerformaceRepository solarPerformaceRepository,IRefreshTokenRepository refreshTokenRepository,
            IAccessRepository accessRepository,IAccessRepository2 accessRepository2 )
        {
            _db = db;
            RoleRepository = roleRepository;
            UserRepository = userRepository;
            TechnicianRepository = technicianRepository;
            AdminRepository = adminRepository;
            ProductsRepository = productsRepository;
            this.category = category;
            this.productReviewRepository = productReviewRepository;
            this.cartRepository = cartRepository;
            this.orderRepository = orderRepository;
            this.homeRepository = homeRepository;
            this.paymentRepository = paymentRepository;
            this.deliveryInterface = deliveryInterface;
            this.invoiceRepository = invoiceRepository;
            this.installRepository = installRepository;
            this.noticationRepositry=noticationRepositry;
            this.maintenanceRepository = maintenanceRepository;
            //this.signalRRepository = signalRRepository;
            this.solarPerformaceRepository = solarPerformaceRepository;
            this.refreshTokenRepository = refreshTokenRepository;
            this.accessRepository = accessRepository;
            this.accessRepository2 = accessRepository2;
        }


        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _db.SaveChangesAsync();
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }

        
    }
}
