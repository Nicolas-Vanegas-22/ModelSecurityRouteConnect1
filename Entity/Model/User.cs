using System;

namespace Entity.Model
{
    public class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public required string Password { get; set; }
        public int? PersonId { get; set; }
        public string? DeleteAt { get; set; }
        public string? CreateAt { get; set; }
    }
}