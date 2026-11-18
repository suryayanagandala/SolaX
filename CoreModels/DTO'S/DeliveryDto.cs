using MimeKit.Tnef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class DeliveryDto
    {
        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public List<ProductImageForDelivery> Products { get; set; } = new List<ProductImageForDelivery>();

        public int Quantity { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string DelievryAddress { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string PaymentMethod {get;set;}=string.Empty;

        public List<string> ImageUrl {  get; set; }=new List<string>();
        public decimal Price {  get; set; }

        public int OrderId {  get; set; }

        public string Status { get; set; } = string.Empty;

        public int DeliveryId { get; set; }

        public DateTimeOffset DeliveryAt { get; set; }

        public string? ProductName { get; set; }

        public string? ProductImage { get; set; }

        public bool IsActive { get; set; }

        public string? InstallationId { get; set; }

    }
}
