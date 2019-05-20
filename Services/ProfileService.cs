using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Api.Data.Entities;
using Api.Data;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace Api.Services
{
  public interface IProfileService
  {
    User GetUser(string userId);
    User CreateUser(string userId, string email);
    void AddProfilePicture(string userId, string avatarUrl);
    List<User> GetAllUsers();
    void UpdateUsername(string userId, string name);
    void UpdateDescription(string userId, string description);
  }

  public class ProfileService : IProfileService
  {
    private readonly DogWalkerContext _dbContext;

    public ProfileService(DogWalkerContext dbContext)
    {
      _dbContext = dbContext;
    }

    public User CreateUser(string userId, string email)
    {
      var user = new User();
      user.Id = userId;
      user.Email = email;
      user.CreationDate = DateTime.UtcNow;

      _dbContext.Users.Add(user);
      _dbContext.SaveChanges();
      return user;
    }

    public User GetUser(string userId)
    {
      var user = _dbContext.Users.Find(userId);
      return user;
    }

    public void AddProfilePicture(string userId, string avatarUrl)
    {
      var userProfile = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
      if (userProfile != null)
      {
        userProfile.AvatarUrl = avatarUrl;
        _dbContext.SaveChanges();
      }
    }

    public List<User> GetAllUsers()
    {
      var users = _dbContext.Users.ToList();
      return users;
    }

    public void UpdateUsername(string userId, string name)
    {
      var userProfile = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
      if (userProfile != null)
      {
        userProfile.Name = name;
        _dbContext.SaveChanges();
      }
    }

    public void UpdateDescription(string userId, string description)
    {
      var userProfile = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
      if (userProfile != null)
      {
        userProfile.Description = description;
        _dbContext.SaveChanges();
      }
    }

  }
}