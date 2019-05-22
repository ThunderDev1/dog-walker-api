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
    public List<int> participantIds { get; set; }
  }
}