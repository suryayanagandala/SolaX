using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService invoiceService;
        public InvoiceController(IUnitOfWork unitOfWork)
        {
            invoiceService=new InvoiceService(unitOfWork);
        }

        [HttpGet("GetInvoice/{orderId}")]
        public async Task<GenericResponse<InvoiceDto>> GetInvoice([FromRoute]int orderId)
        {
            try
            {
                var data=await invoiceService.GetInvoiceData(orderId);
                return new GenericResponse<InvoiceDto> { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                return new GenericResponse<InvoiceDto> { Status=false,Error=ex.Message};
            }
        }

        [HttpGet("invoice/{orderId}")]
        public async Task<GenericResponse<string>> InvoiceResponse(int orderId)
        {
            try
            {
                var pdfBytes = await invoiceService.GeneratePdf(orderId);
                var base64Pdf = Convert.ToBase64String(pdfBytes);

                return new GenericResponse<string>
                {
                    Status = true,
                    Data = base64Pdf,
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<string>
                {
                    Status = false,
                    Error = ex.Message
                };
            }
        }

    }
}
