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

namespace Api.Controllers
{
  [Route("[controller]")]
  public class ProfileController : BaseController
  {
    private readonly IMapper _mapper;
    private readonly IProfileService _profileService;
    private readonly IFileService _fileService;

    public ProfileController(IProfileService profileService, IMapper mapper, IFileService fileService)
    {
      _profileService = profileService;
      _mapper = mapper;
      _fileService = fileService;
    }

    [HttpGet]
    public ActionResult GetUserProfile()
    {
      var user = _profileService.GetUser(UserId);
      if (user == null)
        return Ok(null);
      var profile = _mapper.Map<ProfileBindModel>(user);
      profile.avatarUrl = _fileService.GetSasUri("profilepictures", user.AvatarUrl);
      return Ok(profile);
    }

    [HttpGet("{userId}")]
    [Route("~/profile/{userId}")]
    public ActionResult GetUserProfile(string userId)
    {
      var user = _profileService.GetUser(userId);
      if (user == null)
        return Ok(null);
      var profile = _mapper.Map<PublicProfileBindModel>(user);
      profile.avatarUrl = _fileService.GetSasUri("profilepictures", user.AvatarUrl);
      return Ok(profile);
    }

    [HttpPost]
    public ActionResult CreateUser([FromBody] CreateProfileBindModel model)
    {
      var user = _profileService.CreateUser(UserId, model.email);
      var profile = _mapper.Map<ProfileBindModel>(user);
      return Ok(profile);
    }

    [HttpPost]
    [Route("~/profile/avatar")]
    public async Task<ActionResult> UploadAvatar([FromBody] ImageUploadBindModel model)
    {
      byte[] file = Misc.GetImageFromBase64(model.imageBase64);

      string containerName = "profilepictures";
      if (file.Length > 0)
      {
        string fileName = Misc.GetRandomFileName(model.fileName);

        Uri fileUri = await _fileService.UploadImage(containerName, file, fileName);
        if (fileUri != null)
        {
          _profileService.AddProfilePicture(UserId, fileUri.ToString());
          string sasUri = _fileService.GetSasUri(containerName, fileUri.ToString());
          return Ok(new { avatarUrl = sasUri });
        }
        else
          return Ok("upload failed");
      }
      else
        return Ok("NoFileSelected");
    }

    [HttpGet]
    [Route("~/profile/all")]
    public ActionResult GetAllProfiles()
    {
      var users = _profileService.GetAllUsers();
      if (users == null)
        return Ok(null);

      string sasToken = _fileService.GetSasToken("profilepictures");

      var profiles = users.Select(user => new ProfileBindModel()
      {
        id = user.Id,
        name = user.Name,
        avatarUrl = !string.IsNullOrEmpty(user.AvatarUrl) ? user.AvatarUrl + sasToken : "",
        description = user.Description
      });
      return Ok(profiles);
    }

    [HttpPost]
    [Route("~/profile/update")]
    public ActionResult UpdateProfile([FromBody] ProfileUpdateBindModel model)
    {
      _profileService.UpdateProfile(UserId, model.name, model.description);
      return Ok();
    }

    [HttpPost]
    [Route("~/profile/pushtoken")]
    public ActionResult UpdatePushToken([FromBody] PushTokenBindModel model)
    {
      _profileService.UpdatePushToken(UserId, model.pushToken);
      return Ok();
    }
  }
}