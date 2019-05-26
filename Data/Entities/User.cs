using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Api.Data.Entities
{
  public class User
  {
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public string PushToken { get; set; }
    public DateTime CreationDate { get; set; }

    public ICollection<UserMeeting> UserMeetings { get; } = new List<UserMeeting>();
    public ICollection<Meeting> Meetings { get; } = new List<Meeting>();
  }
}