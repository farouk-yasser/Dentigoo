using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int RecipientUserId { get; set; } //reciver of notification(fk1)
        public int? DentalCaseId { get; set; }  //(fk2)
        public string NotificationType { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set;}
        public bool IsRead { get; set; } 
        public User RecipientUser { get; set; } = null!;
        public DentalCase? DentalCase { get; set; }
    }
}
