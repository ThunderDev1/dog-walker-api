using System;
using System.Collections.Generic;

namespace Api.Data.Entities
{
    public class UserMeeting
    {
        public int Status { get; set; }
        
        public string UserId { get; set; }
        public User User { get; set; }

        public int MeetingId { get; set; }
        public Meeting Meeting { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}