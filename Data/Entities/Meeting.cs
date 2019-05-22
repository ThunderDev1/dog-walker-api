using System;
using System.Collections.Generic;

namespace Api.Data.Entities
{
    public class Meeting
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public string UserId { get; set; }
        public User User { get; set; }

        public int PlaceId { get; set; }
        public Place Place { get; set; }

        public DateTime CreationDate { get; set; }

        public ICollection<UserMeeting> UserMeetings { get; } = new List<UserMeeting>();
    }
}