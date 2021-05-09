using System.ComponentModel.DataAnnotations;

namespace InventoryServiceDemo.DTOs.Requests
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}