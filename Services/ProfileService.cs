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
  }
}