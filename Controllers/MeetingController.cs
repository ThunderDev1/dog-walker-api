
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

    public MeetingController(IMeetingService meetingService, IMapper mapper,IFileService fileService)
    {
      _meetingService = meetingService;
      _mapper = mapper;
      _fileService = fileService;
    }

    [HttpPost]
    [Route("~/meeting")]
    public ActionResult Create([FromBody] CreateMeetingBindModel model)
    {
      var meeting = new MeetingModel();
      meeting.StartDate = DateTime.Now;
      meeting.Duration = new TimeSpan(0, 30, 0);
      meeting.Title = model.title;
      meeting.PlaceId = model.placeId;
      meeting.UserId = UserId;
      meeting.ParticipantIds = model.participantIds;

      int meetingId = _meetingService.Create(meeting);

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
  }
}