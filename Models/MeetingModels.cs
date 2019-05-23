using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Api.Models
{
  public class MeetingModel
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public TimeSpan Duration { get; set; }
    public string UserId { get; set; }
    public int PlaceId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public List<string> ParticipantIds { get; set; }
  }

  public class MeetingItemModel
  {
    public int MeetingId { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PlaceModel Place { get; set; }
    public UserModel Creator { get; set; }
  }

  public class GuestModel
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string AvatarUrl { get; set; }
    public int Status { get; set; }
  }

  public class MeetingDetailsModel
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public UserModel Creator { get; set; }
    public PlaceModel Place { get; set; }
    public DateTime CreationDate { get; set; }
    public List<GuestModel> Guests { get; set; }
    public int CurrentUserStatus { get; set; }
  }
}