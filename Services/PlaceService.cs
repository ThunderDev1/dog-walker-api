using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Api.Data.Entities;
using Api.Data;
using Api.Models;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace Api.Services
{

  public enum PlaceType
  {
    WasteBag = 1,
    Park = 2,
  }

  public interface IPlaceService
  {
    int Create(int placeTypeId, string placeName, string latitude, string longitude);
    List<Place> GetAll();
    void Delete(int placeId);
    List<PlaceModel> GetMeetingPlaces(string latitude, string longitude);
  }

  public class PlaceService : IPlaceService
  {
    private readonly DogWalkerContext _dbContext;

    public PlaceService(DogWalkerContext dbContext)
    {
      _dbContext = dbContext;
    }

    public int Create(int placeTypeId, string placeName, string latitude, string longitude)
    {
      var place = new Place();
      place.CreationDate = DateTime.UtcNow;

      var placeType = _dbContext.PlaceTypes.Find(placeTypeId);
      place.PlaceType = placeType;
      place.Name = placeName;

      var lat = float.Parse(latitude, CultureInfo.InvariantCulture);
      var lng = float.Parse(longitude, CultureInfo.InvariantCulture);

      place.Location = new Point(lng, lat) { SRID = 0 };
      _dbContext.Places.Add(place);
      _dbContext.SaveChanges();
      return place.Id;
    }

    public List<Place> GetAll()
    {
      return _dbContext.Places.ToList();
    }

    public List<PlaceModel> GetMeetingPlaces(string latitude, string longitude)
    {
      var lat = float.Parse(latitude, CultureInfo.InvariantCulture);
      var lng = float.Parse(longitude, CultureInfo.InvariantCulture);
      var origin = new Point(lng, lat) { SRID = 0 };
      var nearestPlaces = _dbContext.Places
      .Where(place => place.PlaceTypeId == (int)PlaceType.Park)
      .OrderBy(place => place.Location.Distance(origin))
      .Select(place => new PlaceModel()
      {
        Id = place.Id,
        Name = place.Name,
        Latitude = place.Location.Y,
        Longitude = place.Location.X,
        CreationDate = place.CreationDate,
        PlaceTypeId = place.PlaceTypeId,
        Distance = place.Location.Distance(origin)
      })
      .Take(10)
      .ToList();
      return nearestPlaces;
    }

    public void Delete(int placeId)
    {
      var place = new Place { Id = placeId };
      _dbContext.Places.Remove(place);
      _dbContext.SaveChanges();
    }
  }
}