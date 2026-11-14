using System;
using System.Collections.Generic;
using System.Text;
using Dentigo.Domain.Enums;
namespace Dentigo.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
       public UserRole Role { get; set; } 
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        }

    }


