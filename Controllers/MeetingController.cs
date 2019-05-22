
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

    public MeetingController(IMeetingService meetingService, IMapper mapper)
    {
      _meetingService = meetingService;
      _mapper = mapper;
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
  }
}