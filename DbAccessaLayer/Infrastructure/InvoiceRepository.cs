using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _db;

        public InvoiceRepository(ApplicationDbContext db)
        {
            _db = db;
        }

       

        public async Task<InvoiceDto> GetInvoice(int orderId)
        {
            try
            {
                var order = await _db.Orders.Include(x => x.Payment.Where(x => x.OrderId == orderId)).Include(x => x.User).Include(x=>x.Invoices)
                    .FirstOrDefaultAsync(x => x.OrderId == orderId);
                if(order == null)
                {
                    throw new NotFoundException("Invalid Order or Order not found");
                }

                var invoiceEntity = order.Invoices.FirstOrDefault()?? throw new NotFoundException("Invoice not found");

                var invoice = new InvoiceDto
                {
                    OrderId = orderId,
                    OrderStatus = order.Status.ToString(),
                    CustomerName = order.FullName,
                    CustomerEmail = order.User.EmailId,
                    PhoneNumber = order.User.PhoneNumber,
                    Tax=order.Invoices.First().Tax,
                    InvoiceDate=DateOnly.FromDateTime(order.Invoices.First().InvoiceDate.DateTime),
                    Payment = order.Payment.Select(x => new PaymentDto
                    {
                        PaymentMethod = x.PaymentMethod,
                        PaymentStatus = x.PaymentStatus,
                        Amount=x.Amount,
                    }).FirstOrDefault()
                };

                return invoice;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> GeneratePdf(int orderId)
        {
            var invoice = await GetInvoice(orderId);
            if (invoice == null)
                throw new Exception("Invalid order");

            //Creates memory buffer to store PDF in memory.
            using var stream = new MemoryStream();

            //PdfWriter writes PDF content into stream.
            var writer = new PdfWriter(stream);

            //Creates PDF document object.
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            document.Add(new Paragraph("INVOICE").SetFontSize(20)
                    .SetFont(boldFont)
                    .SetTextAlignment(TextAlignment.CENTER)
            );

            document.Add(new Paragraph($"Invoice Date: {invoice.InvoiceDate}"));
            document.Add(new Paragraph($"Order Id: {invoice.OrderId}"));

            document.Add(new Paragraph("Customer Details").SetFont(boldFont));

            document.Add(new Paragraph(invoice.CustomerName));
            document.Add(new Paragraph(invoice.CustomerEmail));

            Table table = new Table(2).UseAllAvailableWidth();

            table.AddCell("Amount");
            table.AddCell($"{invoice.Payment?.Amount ?? 0}");

            table.AddCell("Tax");
            table.AddCell($"{invoice.Tax}");

            table.AddCell("Total");
            table.AddCell($"{invoice.Payment?.Amount}");

            document.Add(table);

            document.Add(new Paragraph()
                    .Add(new Text("Payment Status: ").SetFont(boldFont)).Add(invoice.Payment?.PaymentStatus ?? "Pending")
            );

            document.Close();

            //Returns PDF as byte array so controller can return File.
            return stream.ToArray();
        }



    }
}
