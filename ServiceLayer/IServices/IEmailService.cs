using HRS_BussinessLogic.DTOs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRS_ServiceLayer.IServices
{
    public interface IEmailService
    {
        Task<ResponseDTO<InvoiceDTO>> GenerateInvoiceAsync(int reservationId);
        ResponseDTO<byte[]> Generate(InvoiceDTO dto);
        Task<ResponseDTO<object>> SendInvoiceEmailAsync(string toEmail, string guestName, byte[] pdfData, string invoiceNumber);
        Task<ResponseDTO<object>> SendReservationEmailAsync(string toEmail, string guestName);
        Task<ResponseDTO<byte[]>> GetInvoicePdfAsync(int reservationId, bool send);
    }
}
