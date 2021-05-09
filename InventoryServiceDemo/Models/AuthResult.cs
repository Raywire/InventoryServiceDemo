using System;
using InventoryServiceDemo.DTOs.Responses;

namespace InventoryServiceDemo.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public UserInfoReadDto User { get; set; }
    }
}
