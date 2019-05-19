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

namespace Api.Controllers
{
  [Route("[controller]")]
  public class ProfileController : BaseController
  {
    private readonly IMapper _mapper;
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService, IMapper mapper)
    {
      _profileService = profileService;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult GetUserProfile()
    {
      var user = _profileService.GetUser(UserId);
      var profile = _mapper.Map<ProfileBindModel>(user);
      return Ok(profile);
    }

    [HttpPost]
    public ActionResult CreateUser([FromBody] CreateProfileBindModel model)
    {
      var user = _profileService.CreateUser(UserId, model.email);
      var profile = _mapper.Map<ProfileBindModel>(user);
      return Ok(profile);
    }
  }
}