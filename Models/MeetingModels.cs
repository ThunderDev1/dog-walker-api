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
}