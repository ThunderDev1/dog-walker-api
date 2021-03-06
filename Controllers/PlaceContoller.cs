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
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;

namespace Api.Controllers
{
  [Route("[controller]")]
  public class PlaceController : BaseController
  {
    private readonly IMapper _mapper;
    private readonly IPlaceService _placeService;

    public PlaceController(IPlaceService placeService, IMapper mapper)
    {
      _placeService = placeService;
      _mapper = mapper;
    }

    [HttpPost]
    public ActionResult Create([FromBody] CreatePlaceBindModel model)
    {
      int placeId = _placeService.Create(model.placeTypeId, model.placeName, model.latitude, model.longitude);
      return Ok(placeId);
    }

    [HttpGet]
    public ActionResult GetPlaces()
    {
      var features = new List<Feature>();
      var places = _placeService.GetAll();

      foreach (var place in places)
      {
        var point = new Point(new Position(place.Location.Y, place.Location.X));

        var properties = new Dictionary<string, object>();
        properties.Add("id", place.Id);
        properties.Add("name", (String.IsNullOrEmpty(place.Name) ? "" : place.Name));
        properties.Add("placeTypeId", place.PlaceTypeId);

        var feature = new Feature(point, properties);
        features.Add(feature);
      }
      return Json(new FeatureCollection(features));
    }

    [HttpPost]
    [Route("~/place/meeting")]
    public ActionResult GetMeetingPlaces([FromBody] GetPlacesByDistanceBindModel origin)
    {
      var places = _placeService.GetMeetingPlaces(origin.latitude, origin.longitude);
      var model = _mapper.Map<List<PlaceBindModel>>(places);
      return Ok(model);
    }

    [HttpDelete("{placeId}")]
    public ActionResult DeletePlace(int placeId)
    {
        _placeService.Delete(placeId);
        return Ok();
    }

  }
}