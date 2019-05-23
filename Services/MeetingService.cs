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
    List<MeetingItemModel> GetMeetingList(string userId);
    MeetingDetailsModel GetMeeting(string userId, int meetingId);
    List<GuestModel> UpdateStatus(string userId, int meetingId, int status);
  }

  public class MeetingService : IMeetingService
  {
    private readonly IMapper _mapper;
    private readonly DogWalkerContext _dbContext;
    private readonly IFileService _fileService;

    public MeetingService(DogWalkerContext dbContext, IMapper mapper, IFileService fileService)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _fileService = fileService;
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
      meetingDTO.CreationDate = DateTime.Now;

      _dbContext.Meetings.Add(meetingDTO);

      _dbContext.SaveChanges(); //commit to get meeting id

      var attendee = new UserMeeting();
      attendee.MeetingId = meetingDTO.Id;
      attendee.UserId = meeting.UserId;
      attendee.Status = (int)UserMeetingStatus.Going;
      attendee.CreationDate = DateTime.Now;
      attendee.ModificationDate = DateTime.Now;
      _dbContext.UserMeetings.Add(attendee);

      string placeName = _dbContext.Places.Find(meeting.PlaceId).Name;

      var allOtherUsers = _dbContext.Users.Where(user => user.Id != creator.Id).Select(User => User.Id).ToList();

      foreach (var friendId in allOtherUsers)
      {
        attendee = new UserMeeting();
        attendee.MeetingId = meetingDTO.Id;
        attendee.UserId = friendId;
        attendee.Status = (int)UserMeetingStatus.Pending;
        attendee.CreationDate = DateTime.Now;
        attendee.ModificationDate = DateTime.Now;
        _dbContext.UserMeetings.Add(attendee);
      }

      _dbContext.SaveChanges();

      return meetingDTO.Id;
    }

    public List<MeetingItemModel> GetMeetingList(string userId)
    {
      var meetings =
        from meeting in _dbContext.Meetings
        join userMeeting in _dbContext.UserMeetings on meeting.Id equals userMeeting.MeetingId
        where userMeeting.UserId == userId
        select new MeetingItemModel()
        {
          MeetingId = meeting.Id,
          Title = meeting.Title,
          StartDate = meeting.StartDate,
          EndDate = meeting.EndDate,
          Creator = _mapper.Map<UserModel>(meeting.User),
          Place = _mapper.Map<PlaceModel>(meeting.Place),
        };

      return meetings.OrderByDescending(x => x.StartDate).Take(10).ToList();
    }

    public MeetingDetailsModel GetMeeting(string userId, int meetingId)
    {
      var userMeeting = _dbContext.UserMeetings
                          .Include(um => um.Meeting)
                              .ThenInclude(m => m.Place)
                          .Include(um => um.Meeting)
                              .ThenInclude(m => m.UserMeetings)
                                  .ThenInclude(um2 => um2.User)
                          .Include(um => um.Meeting)
                              .ThenInclude(m => m.User)
                          .Include(um => um.User)
                          .Where(um => um.UserId == userId && um.MeetingId == meetingId)
                          .FirstOrDefault();

      var model = new MeetingDetailsModel();

      model.Id = meetingId;
      model.Creator = _mapper.Map<UserModel>(userMeeting.Meeting.User);
      model.StartDate = userMeeting.Meeting.StartDate;
      model.EndDate = userMeeting.Meeting.EndDate;
      model.Title = userMeeting.Meeting.Title;
      model.Place = _mapper.Map<PlaceModel>(userMeeting.Meeting.Place);
      model.CreationDate = userMeeting.Meeting.CreationDate;

      string sasToken = _fileService.GetSasToken("profilepictures");

      model.Guests = (from um in userMeeting.Meeting.UserMeetings
                      select new GuestModel
                      {
                        Id = um.User.Id,
                        Name = um.User.Name,
                        Description = um.User.Description,
                        AvatarUrl = !string.IsNullOrEmpty(um.User.AvatarUrl) ? um.User.AvatarUrl + sasToken : "",
                        Status = um.Status
                      }).ToList();

      model.CurrentUserStatus = userMeeting.Status;

      return model;
    }

    public List<GuestModel> UpdateStatus(string userId, int meetingId, int status)
    {
      var userMeeting = _dbContext.UserMeetings
                          .Include(um => um.Meeting)
                              .ThenInclude(m => m.UserMeetings)
                                  .ThenInclude(um2 => um2.User)
                          .Where(um => um.UserId == userId && um.MeetingId == meetingId)
                          .FirstOrDefault();

      userMeeting.Status = status;
      _dbContext.SaveChanges();

      string sasToken = _fileService.GetSasToken("profilepictures");

      // get updated guest list
      var attendees = (from um in userMeeting.Meeting.UserMeetings
                       select new GuestModel
                       {
                         Id = um.User.Id,
                         Name = um.User.Name,
                         Description = um.User.Description,
                         AvatarUrl = !string.IsNullOrEmpty(um.User.AvatarUrl) ? um.User.AvatarUrl + sasToken : "",
                         Status = um.Status
                       }).ToList();

      return attendees;
    }
  }
}