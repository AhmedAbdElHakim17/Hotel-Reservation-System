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
        Task<ResponseDTO<RoomGetDTO>> AddRoomAsync(RoomGetDTO roomDTO);
        Task<ResponseDTO<RoomGetDTO>> UpdateRoomAsync(int id, RoomGetDTO roomDTO);
        Task<ResponseDTO<RoomGetDTO>> DeleteRoomAsync(int id);
    }
}
