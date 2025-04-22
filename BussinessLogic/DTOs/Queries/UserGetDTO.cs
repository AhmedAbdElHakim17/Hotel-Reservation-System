using Microsoft.AspNetCore.Identity;

namespace HRS_BussinessLogic.DTOs.Queries
{
    public class UserGetDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = "No Role";
    }
}
