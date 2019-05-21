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
  public interface IPlaceService
  {
    int Create(int placeTypeId, string placeName, string latitude, string longitude);
    List<Place> GetAll();
    void Delete(int placeId);
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

    public void Delete(int placeId)
    {
      var place = new Place { Id = placeId };
      _dbContext.Places.Remove(place);
      _dbContext.SaveChanges();
    }
  }
}