using System;

namespace Diplomka.Models
{
    public class Users
    {
        public string? Id { get; set; }
        public string? RoleId { get; set; }
        public string? AuthenticationId { get; set; }
        public Authentications? Authentications { get; set; }
        public Roles? Roles { get; set; }
        public HREmployees? HREmployees { get; set; }
    }
}
