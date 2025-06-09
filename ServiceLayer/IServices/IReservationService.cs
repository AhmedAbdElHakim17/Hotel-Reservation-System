using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using System.Security.Claims;

namespace HRS_ServiceLayer.IServices
{
    public interface IReservationService
    {
        Task<ResponseDTO<List<ReservationGetDTO>>> GetAllReservationsAsync();
        Task<ResponseDTO<List<ReservationGetDTO>>> GetUpcomingReservationsAsync();

        Task<ResponseDTO<List<ReservationGetDTO>>> GetAllUserReservationsAsync(ClaimsPrincipal user);
        Task<ResponseDTO<List<ReservationGetDTO>>> GetUserUpcomingReservationsAsync(ClaimsPrincipal user);
        Task<ResponseDTO<ReservationGetDTO>> GetReservationById(int id);
        Task<ResponseDTO<ReservationGetDTO>> AddReservationAsync(ReservationPostDTO reservationDTO, ClaimsPrincipal user);


        Task<ResponseDTO<ReservationGetDTO>> UpdateReservationAsync(ReservationPostDTO reservationDTO, int id);

        Task<ResponseDTO<ReservationGetDTO>> CancelReservationAsync(int id);
        Task<ResponseDTO<ReservationGetDTO>> CheckInReservationAsync(int id, ClaimsPrincipal claims);
        Task<ResponseDTO<ReservationGetDTO>> CheckOutReservationAsync(int id, ClaimsPrincipal claims);
        Task<ResponseDTO<ReservationGetDTO>> ConfirmReservationAsync(int id, ClaimsPrincipal claims);
        

    }
}
