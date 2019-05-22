
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Api.Models
{
  public class PlaceModel
  {
    public int    Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreationDate { get; set; }
    public int    PlaceTypeId { get; set; }
    public double Distance { get; set; }
  }
}