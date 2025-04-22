using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;

namespace HRS_ServiceLayer.IServices
{
    public interface IReservationService
    {
        Task<ResponseDTO<List<ReservationGetDTO>>> GetAllReservationsAsync();
        Task<ResponseDTO<List<ReservationGetDTO>>> GetUpcomingReservationsAsync();

        Task<ResponseDTO<List<ReservationGetDTO>>> GetAllUserReservationsAsync(AppUser user);
        Task<ResponseDTO<List<ReservationGetDTO>>> GetUserUpcomingReservationsAsync(AppUser user);
        Task<ResponseDTO<ReservationGetDTO>> AddReservationAsync(ReservationPostDTO reservationDTO);


        Task<ResponseDTO<ReservationGetDTO>> UpdateReservationAsync(ReservationPostDTO reservationDTO, int id);

        Task<ResponseDTO<ReservationGetDTO>> CancelReservationAsync(int id);
        Task<ResponseDTO<ReservationGetDTO>> CheckInReservationAsync(int id);
        Task<ResponseDTO<ReservationGetDTO>> CheckOutReservationAsync(int id);
        Task<ResponseDTO<ReservationGetDTO>> ConfirmReservationAsync(int id);
        

    }
}
