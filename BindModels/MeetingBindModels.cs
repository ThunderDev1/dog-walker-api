using System;
using System.Collections.Generic;

namespace Api.BindModels
{
  public class GuestBindModel
  {
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string avatarUrl { get; set; }
    public int status { get; set; }
  }

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
    public int meetingId { get; set; }
    public string title { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
    public string placeName { get; set; }
    public string creatorName { get; set; }
    public string creatorAvatarUrl { get; set; }
  }

  public class MeetingDetailsBindModel
  {
    public int meetingId { get; set; }
    public string creatorName { get; set; }
    public string creatorAvatarUrl { get; set; }
    public string title { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
    public int status { get; set; }
    public string creationDate { get; set; }
    public bool isCreator { get; set; }
    public int placeTypeId { get; set; }
    public int placeId { get; set; }
    public List<GuestBindModel> guests { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
  }

  public class MeetingPresenceBindModel
  {
    public int meetingId { get; set; }
    public int status { get; set; }
  }
}