using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Newtonsoft.Json;
using System.Dynamic;
using System.Threading.Tasks;
using Api.Data.Entities;
using Api.Models;
using Api.Data;

namespace Api.Services
{
  public enum UserMeetingStatus
  {
    Pending = 1,
    Going = 2,
    NotGoing = 3
  }

  public interface IMeetingService
  {
    int Create(MeetingModel meeting);
  }

  public class MeetingService : IMeetingService
  {
    private readonly IMapper _mapper;
    private readonly DogWalkerContext _dbContext;

    public MeetingService(DogWalkerContext dbContext, IMapper mapper)
    {
      _dbContext = dbContext;
      _mapper = mapper;
    }

    public int Create(MeetingModel meeting)
    {
      var creator = _dbContext.Users.Find(meeting.UserId);
      string title = "";

      // create default title if none specified
      if (string.IsNullOrEmpty(meeting.Title))
        title = "Balade avec " + creator.Name;
      else
        title = meeting.Title;

      var meetingDTO = new Meeting();

      meetingDTO.PlaceId = meeting.PlaceId;
      meetingDTO.UserId = meeting.UserId;
      meetingDTO.Title = title;
      meetingDTO.StartDate = meeting.StartDate;
      meetingDTO.EndDate = meeting.StartDate + meeting.Duration;
      meetingDTO.CreationDate = DateTime.UtcNow;

      _dbContext.Meetings.Add(meetingDTO);

      _dbContext.SaveChanges(); //commit to get meeting id

      var attendee = new UserMeeting();
      attendee.MeetingId = meetingDTO.Id;
      attendee.UserId = meeting.UserId;
      attendee.Status = (int)UserMeetingStatus.Going;
      attendee.CreationDate = DateTime.UtcNow;
      attendee.ModificationDate = DateTime.UtcNow;
      _dbContext.UserMeetings.Add(attendee);


      string placeName = _dbContext.Places.Find(meeting.PlaceId).Name;

      var allOtherUsers = _dbContext.Users.Where(user => user.Id != creator.Id).Select(User => User.Id).ToList();

      foreach (var friendId in allOtherUsers)
      {
        attendee = new UserMeeting();
        attendee.MeetingId = meetingDTO.Id;
        attendee.UserId = friendId;
        attendee.Status = (int)UserMeetingStatus.Pending;
        attendee.CreationDate = DateTime.UtcNow;
        attendee.ModificationDate = DateTime.UtcNow;
        _dbContext.UserMeetings.Add(attendee);
      }

      _dbContext.SaveChanges();

      return meetingDTO.Id;
    }
  }
}