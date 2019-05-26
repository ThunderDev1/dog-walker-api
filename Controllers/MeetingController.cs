
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Api.BindModels;
using AutoMapper;
using System.Globalization;
using System.Dynamic;
using System.IO;
using Api.Data.Entities;
using Newtonsoft.Json;
using Api.Utilities;
using Api.Models;

namespace Api.Controllers
{
  [Route("[controller]")]
  public class MeetingController : BaseController
  {
    private readonly IMapper _mapper;
    private readonly IMeetingService _meetingService;
    private readonly IFileService _fileService;

    public MeetingController(IMeetingService meetingService, IMapper mapper, IFileService fileService)
    {
      _meetingService = meetingService;
      _mapper = mapper;
      _fileService = fileService;
    }

    [HttpPost]
    [Route("~/meeting")]
    public async Task<ActionResult> Create([FromBody] CreateMeetingBindModel model)
    {
      var meeting = new MeetingModel();
      meeting.StartDate = DateTime.UtcNow;
      meeting.Duration = new TimeSpan(0, 30, 0);
      meeting.Title = model.title;
      meeting.PlaceId = model.placeId;
      meeting.UserId = UserId;
      meeting.ParticipantIds = model.participantIds;

      int meetingId = await _meetingService.Create(meeting);

      return Ok(new { meetingId = meetingId });
    }

    [HttpGet]
    [Route("~/meetings")]
    public ActionResult GetMeetings()
    {
      var meetings = _meetingService.GetMeetingList(UserId);
      var bindModel = new List<MeetingItemBindModel>();

      MeetingItemBindModel meetingBindModel;
      foreach (var meeting in meetings)
      {
        meetingBindModel = new MeetingItemBindModel();
        meetingBindModel.meetingId = meeting.MeetingId;
        meetingBindModel.title = meeting.Title;
        meetingBindModel.startDate = meeting.StartDate.ToString("o", CultureInfo.InvariantCulture);
        meetingBindModel.endDate = meeting.EndDate.ToString("o", CultureInfo.InvariantCulture);
        meetingBindModel.placeName = meeting.Place.Name;
        meetingBindModel.creatorName = meeting.Creator.Name;
        meetingBindModel.creatorAvatarUrl = _fileService.GetSasUri("profilepictures", meeting.Creator.AvatarUrl);
        bindModel.Add(meetingBindModel);
      }

      return Ok(bindModel);
    }

    [HttpGet("{meetingId}")]
    public ActionResult GetMeetingDetails(int meetingId)
    {
      MeetingDetailsModel meeting = _meetingService.GetMeeting(UserId, meetingId);
      var model = new MeetingDetailsBindModel();

      model.meetingId = meeting.Id;
      model.title = meeting.Title;
      model.status = meeting.CurrentUserStatus;
      model.creatorName = meeting.Creator.Name;
      model.creatorAvatarUrl = _fileService.GetSasUri("profilepictures", meeting.Creator.AvatarUrl);
      model.startDate = meeting.StartDate.ToString("o", CultureInfo.InvariantCulture);
      model.endDate = meeting.EndDate.ToString("o", CultureInfo.InvariantCulture);
      model.creationDate = meeting.CreationDate.ToString("o", CultureInfo.InvariantCulture);
      model.guests = _mapper.Map<List<GuestBindModel>>(meeting.Guests);

      model.isCreator = (meeting.Creator.Id == UserId);

      model.placeId = meeting.Place.Id;
      model.placeTypeId = meeting.Place.PlaceTypeId;

      model.latitude = meeting.Place.Latitude;
      model.longitude = meeting.Place.Longitude;

      return Ok(model);
    }

    [HttpPost]
    [Route("~/meeting/presence")]
    public ActionResult UpdateStatus([FromBody] MeetingPresenceBindModel model)
    {
      List<GuestModel> attendees = _meetingService.UpdateStatus(UserId, model.meetingId, model.status);
      var guests = _mapper.Map<List<GuestBindModel>>(attendees);
      return Ok(guests);
    }

    [HttpGet]
    [Route("~/meeting/ongoing")]
    public ActionResult GetOnGoingMeeting()
    {
      var onGoingMeetingId = _meetingService.GetOnGoingMeeting(UserId);
      return Ok(onGoingMeetingId);
    }

    [HttpGet("~/meeting/cancel/{meetingId}")]
    public ActionResult CancelMeeting(int meetingId)
    {
      _meetingService.CancelMeeting(UserId, meetingId);
      return Ok();
    }
  }
}