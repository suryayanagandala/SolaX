using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        public IRoleRepository RoleRepository { get; }

        public IUserRepository UserRepository { get; }

        public ITechnicianRepository TechnicianRepository { get; }

        public IAdminRepository AdminRepository { get; }

        public IProductsRepository ProductsRepository { get; }

        //public IOtpServiceRepository otpServiceRepository { get; }

        public ICategoryRepository category { get; }

        public IProductReviewRepository productReviewRepository { get; }

        public ICartRepository cartRepository { get; }

        public IOrderRepository orderRepository { get; }

        public IHomeRepository homeRepository { get; }

        public IPaymentRepository paymentRepository { get; }

        public IDeliveryInterface deliveryInterface { get; }

        public IInvoiceRepository invoiceRepository { get; }

        public IInstallRepository installRepository { get; }

        public INoticationRepositry noticationRepositry { get; }

        public IMaintenanceRepository maintenanceRepository { get; }

        //public ISignalRRepository signalRRepository { get; }

        public ISolarPerformaceRepository solarPerformaceRepository { get; }

        public IRefreshTokenRepository refreshTokenRepository { get; }

        public IAccessRepository accessRepository { get; }

        public IAccessRepository2 accessRepository2 { get; }

        public Task<int> SaveAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

    }
}
