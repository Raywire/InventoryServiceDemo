using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryServiceDemo.DTOs.Requests
{
    public class UserRegisterDto
    {
        [Required]
        [MaxLength(40)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(40)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(64)]
        public string Email { get; set; }

        [Required]
        [MaxLength(32)]
        public string Password { get; set; }
    }
}
