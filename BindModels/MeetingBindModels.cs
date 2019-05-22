using System;
using System.Collections.Generic;

namespace Api.BindModels
{
  public class CreateMeetingBindModel
  {
    public int placeId { get; set; }
    public string title { get; set; }
    public string startDate { get; set; }
    public TimeSpan duration { get; set; }
    public List<string> participantIds { get; set; }
  }

  public class MeetingItemBindModel
    {
        public int    meetingId   { get; set; } 
        public string title       { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string placeName   { get; set; }
        public string creatorName { get; set; }
        public string creatorAvatarUrl { get; set; }
    }
}