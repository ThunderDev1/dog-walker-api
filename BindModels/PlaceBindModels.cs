using System.Collections.Generic;

namespace Api.BindModels
{
  public class CreatePlaceBindModel
  {
    public int placeTypeId { get; set; }
    public string longitude { get; set; }
    public string latitude { get; set; }
  }

  public class MapPlaceBindModel
  {
    public int placeTypeId { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
  }
}