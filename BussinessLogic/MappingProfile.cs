using AutoMapper;
using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using HRS_BussinessLogic.Models;
using HRS_DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace BussinessLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<AppUser, UserGetDTO>().ReverseMap();
            CreateMap<AppUser, UserPostDTO>().ReverseMap();
            // Role
            CreateMap<IdentityRole, RoleDTO>().ReverseMap();
            // Rooms
            CreateMap<Room, RoomGetDTO>().ReverseMap();
            CreateMap<Room, RoomPostDTO>().ReverseMap();

            // Reservations
            CreateMap<Reservation, ReservationPostDTO>()
                .ForMember(dest => dest.RoomNum, src => src.MapFrom(src => src.Room.RoomNum))
                .ReverseMap();

            CreateMap<Reservation, ReservationGetDTO>()
                .ForMember(dest => dest.RoomNum, src => src.MapFrom(src => src.Room.RoomNum))
                .ForMember(dest => dest.UserName, src => src.MapFrom(src => src.User.UserName != null ? src.User.UserName : null))
                .ReverseMap();

            CreateMap<ReservationGetDTO, ReservationPostDTO>()
                .ReverseMap();
            // Feedback
            CreateMap<Feedback, FeedbackDTO>()
                .ForMember(dest => dest.UserName, src => src.MapFrom(src => src.User.UserName != null ? src.User.UserName : null))
                .ReverseMap();
            // Offer
            CreateMap<Offer, OfferDTO>().ReverseMap();
            // Payment
            CreateMap<Payment, PaymentDTO>().ReverseMap();
        }
    }
}
