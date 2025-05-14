using HRS_BussinessLogic.Attributes;
using HRS_SharedLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRS_BussinessLogic.DTOs.Commands
{
    public class RoomPostDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Room number is required")]
        [UniqueRoomNumber] 
        [Range(1,200,ErrorMessage ="Room number must be between 1 and 200")]
        public int RoomNum { get; set; }
        [Required(ErrorMessage ="Price Per Night is required")]
        [Range(100,10000,ErrorMessage ="Price must range between 100 and 1000")]
        public double PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        [Required(ErrorMessage ="Image Url is required")]
        //[RegularExpression(@"\w+\.(jpg|png)")]
        public string ImageUrl { get; set; }
        [MaxLength(500, ErrorMessage = "Facilities description cannot exceed 500 characters.")]
        public string Facilities { get; set; }
        [Required(ErrorMessage ="Room Type is required")]
        public RoomType RoomType { get; set; }
    }
}
