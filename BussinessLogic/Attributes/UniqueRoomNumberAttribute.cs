using HRS_BussinessLogic.DTOs.Queries;
using HRS_DataAccess.Models;
using HRS_SharedLayer.Interfaces.IBases;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace HRS_BussinessLogic.Attributes
{
    public class UniqueRoomNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var validator = validationContext.GetService<IBaseValidator<Room>>();
            var room = (RoomGetDTO)validationContext.ObjectInstance;
            if (!validator.IsRoomNumberUnique(room.RoomNum,room.Id))
                return new ValidationResult("Room Number already Exists");
            return ValidationResult.Success;
        }
    }
}
