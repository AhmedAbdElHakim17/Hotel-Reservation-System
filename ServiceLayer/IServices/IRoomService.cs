using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRS_ServiceLayer.IServices
{
    public interface IRoomService
    {
        Task<ResponseDTO<List<RoomGetDTO>>> GetAllRoomsAsync();
        Task<ResponseDTO<List<RoomGetDTO>>> GetAvailableRoomsAsync(DateTime from, DateTime to);
        Task<ResponseDTO<RoomGetDTO>> GetRoomByIdAsync(int id);
        Task<ResponseDTO<RoomPostDTO>> AddRoomAsync(RoomPostDTO roomDTO);
        Task<ResponseDTO<RoomPostDTO>> UpdateRoomAsync(int id, RoomPostDTO roomDTO);
        Task<ResponseDTO<RoomGetDTO>> DeleteRoomAsync(int id);
    }
}
