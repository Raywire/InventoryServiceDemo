using System;
using System.Collections.Generic;

namespace InventoryServiceDemo.DTOs.Responses
{
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
