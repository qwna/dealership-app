using System;
namespace Dealership.Models
{
    public class User
    {
        public int Id { get; set; }
        public string DealerCode { get; set; } = "";
        public string FullName { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool IsAdmin { get; set; }
    }
}

