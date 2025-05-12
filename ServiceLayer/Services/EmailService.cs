using Hangfire;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using HRS_DataAccess;
using HRS_ServiceLayer.IServices;
using HRS_SharedLayer.Enums;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace HRS_ServiceLayer.Services
{
    public class EmailService(IUnitOfWork unitOfWork, IConfiguration configuration) : IEmailService
    {
        public async Task<ResponseDTO<InvoiceDTO>> GenerateInvoiceAsync(int reservationId)
        {
            try
            {
                var reservation = await unitOfWork.Reservations
                .FindAsync(r => r.Id == reservationId,
                    nameof(Reservation.Room),
                    nameof(Reservation.User));
                var payment = await unitOfWork.Payments.FindAsync(p => p.ReservationId == reservationId);
                if (reservation == null || reservation.ReservationStatus != ReservationStatus.Confirmed
                    || payment.PaymentStatus != PaymentStatus.Paid)
                    return new ResponseDTO<InvoiceDTO>("Reservation not found or not completed", null);

                var nights = (reservation.CheckOutDate - reservation.CheckInDate).Days;
                var room = reservation.Room;
                var discount = room.PricePerNight * nights - (double)reservation.TotalAmount;
                var invoice = new InvoiceDTO
                {
                    InvoiceNumber = $"INV-{reservation.Id:D6}",
                    GuestName = reservation.User.UserName,
                    GuestEmail = reservation.User.Email,
                    RoomNumber = room.RoomNum,
                    RoomType = (RoomType)room.RoomType,
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    NumberOfNights = nights,
                    RatePerNight = room.PricePerNight,
                    Discount = $"{discount}",
                    TotalAmount = reservation.TotalAmount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentDate = payment?.TransactionDate ?? DateTime.Now
                };
                return new ResponseDTO<InvoiceDTO>("Invoice Pdf is Created successfully", invoice);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<InvoiceDTO>("Failed to generate Invoice", null);
            }

        }
        public ResponseDTO<byte[]> Generate(InvoiceDTO dto)
        {
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                           .Row(row =>
                           {
                               row.RelativeItem().Column(col =>
                               {
                                   col.Item().Text("Hotel Management System").Bold().FontSize(20).FontColor(Colors.Blue.Medium);
                                   col.Item().Text("123 Main Street, Cairo, Egypt").FontSize(10);
                                   col.Item().Text("Email: contact@hotel.com").FontSize(10);
                               });
                           });

                        page.Content()
                            .Column(col =>
                            {
                                col.Spacing(10);

                                col.Item().Text($"Invoice #: {dto.InvoiceNumber}").Bold();
                                col.Item().Text($"Invoice Date: {DateTime.Now:yyyy-MM-dd}");
                                col.Item().Text("");

                                col.Item().Text("Guest Information").Bold();
                                col.Item().Text($"Name: {dto.GuestName}");
                                col.Item().Text($"Email: {dto.GuestEmail}");
                                col.Item().Text("");

                                col.Item().Text("Reservation Details").Bold();
                                col.Item().Text($"Room: {dto.RoomNumber} ({dto.RoomType})");
                                col.Item().Text($"Check-In: {dto.CheckInDate:yyyy-MM-dd}");
                                col.Item().Text($"Check-Out: {dto.CheckOutDate:yyyy-MM-dd}");
                                col.Item().Text($"Nights: {dto.NumberOfNights}");
                                col.Item().Text($"Rate per Night: ${dto.RatePerNight:N2}");
                                col.Item().Text("");

                                col.Item().Text("Charges").Bold();
                                col.Item().Text($"Discount: -${dto.Discount:N2}");
                                col.Item().Text($"Total Paid: ${dto.TotalAmount:N2}").Bold();
                                col.Item().Text("");

                                col.Item().Text("Payment Information").Bold();
                                col.Item().Text($"Method: {dto.PaymentMethod}");
                                col.Item().Text($"Paid on: {dto.PaymentDate:yyyy-MM-dd}");
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text("You are Welcome in our hotel any time").FontSize(10);
                    });
                }).GeneratePdf();
                return new ResponseDTO<byte[]>("Invoice generated successfully", document);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<byte[]>($"Error: {ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<object>> SendInvoiceEmailAsync(string toEmail, string guestName, byte[] pdfData, string invoiceNumber)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(configuration["EmailSettings:From"]));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = $"Your Hotel Invoice #{invoiceNumber}";

                var builder = new BodyBuilder();

                builder.HtmlBody = $@"
            <p>Dear {guestName},</p>
            <p>Thank you for your stay at our hotel. Please find your invoice #{invoiceNumber} attached.</p>
            <p>If you have any questions, feel free to contact us.</p>
            <br />
            <p>Best regards,<br />Hotel Management</p>
        ";

                builder.Attachments.Add($"Invoice-{invoiceNumber}.pdf", pdfData, new ContentType("application", "pdf"));
                message.Body = builder.ToMessageBody();
                using var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(configuration["EmailSettings:SmtpHost"], int.Parse(configuration["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(configuration["EmailSettings:Username"], configuration["EmailSettings:Password"]);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
                return new ResponseDTO<object>("Email Sent successfully", new object { });
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>($"Error:{ex.Message}", null);
            }
        }
        public async Task<ResponseDTO<object>> SendReservationEmailAsync(string toEmail, string guestName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(configuration["EmailSettings:From"]));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = $"Your Reservation Confirmation";

                var builder = new BodyBuilder();

                builder.HtmlBody = $@"
            <p>Dear {guestName},</p>
            <p>Thank you for making a reservation in our hotel, Please pay for it to Cofirm the Process</p>
            <p>If you have any questions, feel free to contact us.</p>
            <br />
            <p>Best regards,<br />Hotel Management</p>";
                message.Body = builder.ToMessageBody();
                using var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(configuration["EmailSettings:SmtpHost"], int.Parse(configuration["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(configuration["EmailSettings:Username"], configuration["EmailSettings:Password"]);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
                return new ResponseDTO<object>("Email sent successfully", new object { });
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>($"Error: {ex.Message}", null);
            }
        }

        public async Task<ResponseDTO<byte[]>> GetInvoicePdfAsync(int reservationId)
        {
            try
            {
                var response = await GenerateInvoiceAsync(reservationId);
                if (!response.IsSuccess)
                    return new ResponseDTO<byte[]>("Invoice Generation failed because of Invalid Reservation Id", null);
                var invoice = response.Data;
                var response1 = Generate(invoice);
                var pdfBytes = response1.Data;

                BackgroundJob.Enqueue<IEmailService>(email =>
                            email.SendInvoiceEmailAsync(
                            invoice.GuestEmail,
                            invoice.GuestName,
                            pdfBytes,
                            invoice.InvoiceNumber
                        ));
                return new ResponseDTO<byte[]>("Invoice Generation succedded", pdfBytes);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<byte[]>($"Error: {ex.Message}", null);
            }
        }

    }
}
