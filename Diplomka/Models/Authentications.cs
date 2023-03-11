using System;

namespace Diplomka.Models
{
    public class Authentications
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? LoginName { get; set; }
        public string? Password { get; set; }
        public Users? Users { get; set; }
    }
}
