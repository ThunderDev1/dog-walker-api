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
using Api.Settings;
using Microsoft.Extensions.Options;
using FCM.Net;

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
    Task<int> Create(MeetingModel meeting);
    List<MeetingItemModel> GetMeetingList(string userId);
    MeetingDetailsModel GetMeeting(string userId, int meetingId);
    List<GuestModel> UpdateStatus(string userId, int meetingId, int status);
    int GetOnGoingMeeting(string userId);
    void CancelMeeting(string userId, int meetingId);
  }

  public class MeetingService : IMeetingService
  {
    private readonly IMapper _mapper;
    private readonly DogWalkerContext _dbContext;
    private readonly IFileService _fileService;
    private readonly NotificationSettings _notificationSettings;
    private readonly EndpointSettings _endpointSettings;

    public MeetingService(DogWalkerContext dbContext, IMapper mapper, IFileService fileService, IOptions<NotificationSettings> notificationOptions, IOptions<EndpointSettings> endpointOptions)
    {
      _dbContext = dbContext;
      _mapper = mapper;
      _fileService = fileService;
      _notificationSettings = notificationOptions.Value;
      _endpointSettings = endpointOptions.Value;
    }

    public async Task<int> Create(MeetingModel meeting)
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

      var allOtherUsers = _dbContext.Users.Where(user => user.Id != creator.Id).ToList();

      foreach (var friend in allOtherUsers)
      {
        attendee = new UserMeeting();
        attendee.MeetingId = meetingDTO.Id;
        attendee.UserId = friend.Id;
        attendee.Status = (int)UserMeetingStatus.Pending;
        attendee.CreationDate = DateTime.UtcNow;
        attendee.ModificationDate = DateTime.UtcNow;
        _dbContext.UserMeetings.Add(attendee);
      }

      using (var sender = new Sender(_notificationSettings.FirebaseServerKey))
      {
        var registrationIds = allOtherUsers
        .Where(user => !string.IsNullOrEmpty(user.PushToken))
        .Select(user => user.PushToken)
        .ToList();

        var message = new Message
        {
          RegistrationIds = registrationIds,
          Notification = new Notification
          {
            Title = creator.Name + " est parti en balade",
            Body = "Cliquez ici pour rejoindre la balade",
            ClickAction = _endpointSettings.Spa + "/#/meeting/" + meetingDTO.Id,
            Icon = _notificationSettings.IconUrl,
            Badge = _notificationSettings.BadgeUrl
          }
        };
        var result = await sender.SendAsync(message);
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

    public int GetOnGoingMeeting(string userId)
    {
      var onGoingMeeting = _dbContext.UserMeetings
      .Where(um => um.UserId == userId
      && um.Status == (int)UserMeetingStatus.Going
      && um.Meeting.StartDate < DateTime.UtcNow
      && um.Meeting.EndDate > DateTime.UtcNow).FirstOrDefault();

      if (onGoingMeeting != null)
        return onGoingMeeting.MeetingId;
      else
        return 0;
    }

    public void CancelMeeting(string userId, int meetingId)
    {
      var onGoingMeeting = _dbContext.Meetings
      .Where(meeting => meeting.Id == meetingId && meeting.UserId == userId)
      .FirstOrDefault();
      if (onGoingMeeting != null)
      {
        onGoingMeeting.EndDate = DateTime.MinValue;
        _dbContext.SaveChanges();
      }
    }
  }
}