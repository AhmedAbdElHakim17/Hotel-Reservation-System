using HRS_BussinessLogic.DTOs.Commands;
using HRS_BussinessLogic.DTOs.Queries;
using System.ComponentModel.DataAnnotations;
namespace HRS_BussinessLogic.Attributes
{
    public class ValidationDatesAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is ReservationPostDTO ResDTO)
            {
                if (ResDTO.CheckInDate < DateTime.Now) 
                    return new ValidationResult("Check-In Date can't be in the past");
                if (ResDTO.CheckInDate >= ResDTO.CheckOutDate) 
                    return new ValidationResult("Check-Out Date must be after Check_In Date");
                return ValidationResult.Success;
            }
            else if (validationContext.ObjectInstance is OfferDTO offerDTO)
            {
                if(offerDTO.StartDate < DateOnly.FromDateTime(DateTime.Now)) 
                    return new ValidationResult("Offer Start Date can't be in the past");
                if (offerDTO.EndDate <= offerDTO.StartDate)
                    return new ValidationResult("EndDate must be after StartDate");
                return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }
    }
}

