using HRS_BussinessLogic.Attributes;
using HRS_SharedLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRS_BussinessLogic.DTOs.Queries
{
    public class OfferDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Offer title is required")]
        [StringLength(100,ErrorMessage ="Title can't exceed 100 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Offer description is required")]
        [StringLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Offer discount is required")]
        [Range(0.01,100,ErrorMessage ="Discount must be between 0.01% and 100%")]
        public double Discount { get; set; }
        [Required(ErrorMessage = "Offer Start date is required")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "Offer end date is required")]
        [ValidationDates]
        public DateOnly EndDate { get; set; }
        [Required(ErrorMessage ="Room Type is required")]
        public RoomType RoomType { get; set; }
    }
}
