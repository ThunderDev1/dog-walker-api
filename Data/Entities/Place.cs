using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Entities
{
  public class Place
  {
    public int Id { get; set; }
    public string Name { get; set; }
    [Column(TypeName = "geometry")]
    public Point Location { get; set; }
    public DateTime CreationDate { get; set; }
    public int PlaceTypeId { get; set; }
    public PlaceType PlaceType { get; set; }
  }
}