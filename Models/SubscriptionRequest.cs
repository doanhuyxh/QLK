using System;

namespace AMS.Models
{
    public class SubscriptionRequest : EntityBase
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string TimeZone { get; set; }
    }
}
