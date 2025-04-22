using AutoMapper;
using HRS_DataAccess.Models;
using HRS_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRS_ServiceLayer.IServices;
using HRS_BussinessLogic.DTOs.Queries;

namespace HRS_ServiceLayer.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ResponseDTO<List<RoomGetDTO>>> GetAllRoomsAsync()
        {
            var rooms = await unitOfWork.Rooms.GetAllAsync();
            if (rooms == null || rooms.Count == 0)
                return new ResponseDTO<List<RoomGetDTO>>("No rooms found", null);

            var roomDTOs = mapper.Map<List<RoomGetDTO>>(rooms);
            return new ResponseDTO<List<RoomGetDTO>>("Rooms retrieved successfully", roomDTOs);
        }

        public async Task<ResponseDTO<List<RoomGetDTO>>> GetAvailableRoomsAsync(DateTime from, DateTime to)
        {
            var availableRooms = await unitOfWork.Rooms.GetAllAvailableAsync(from, to);
            if (availableRooms == null || availableRooms.Count == 0)
                return new ResponseDTO<List<RoomGetDTO>>("No available rooms found", null);

            var roomDTOs = mapper.Map<List<RoomGetDTO>>(availableRooms);
            return new ResponseDTO<List<RoomGetDTO>>("Available rooms retrieved successfully", roomDTOs);
        }

        public async Task<ResponseDTO<RoomGetDTO>> GetRoomByIdAsync(int id)
        {
            var room = await unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                return new ResponseDTO<RoomGetDTO>("Room not found", null);

            var roomDTO = mapper.Map<RoomGetDTO>(room);
            return new ResponseDTO<RoomGetDTO>("Room retrieved successfully", roomDTO);
        }

        public async Task<ResponseDTO<RoomGetDTO>> AddRoomAsync(RoomGetDTO roomDTO)
        {
            var room = mapper.Map<Room>(roomDTO);
            await unitOfWork.Rooms.AddAsync(room);
            await unitOfWork.CompleteAsync();

            var resultDTO = mapper.Map<RoomGetDTO>(room);
            return new ResponseDTO<RoomGetDTO>("Room added successfully", resultDTO);
        }

        public async Task<ResponseDTO<RoomGetDTO>> UpdateRoomAsync(int id, RoomGetDTO roomDTO)
        {
            var room = await unitOfWork.Rooms.FindAsync(r => r.Id == id);
            if (room == null)
                return new ResponseDTO<RoomGetDTO>("Room not found", null);

            if (roomDTO.Id != id)
                return new ResponseDTO<RoomGetDTO>("ID mismatch", null);

            mapper.Map(roomDTO, room);
            unitOfWork.Rooms.Update(room);
            await unitOfWork.CompleteAsync();

            var resultDTO = mapper.Map<RoomGetDTO>(room);
            return new ResponseDTO<RoomGetDTO>("Room updated successfully", resultDTO);
        }

        public async Task<ResponseDTO<RoomGetDTO>> DeleteRoomAsync(int id)
        {
            var room = await unitOfWork.Rooms.FindAsync(r => r.Id == id);
            if (room == null)
                return new ResponseDTO<RoomGetDTO>("Room not found", null);

            unitOfWork.Rooms.Delete(room);
            await unitOfWork.CompleteAsync();

            var resultDTO = mapper.Map<RoomGetDTO>(room);
            return new ResponseDTO<RoomGetDTO>("Room deleted successfully", resultDTO);
        }
    }
}
