namespace Api.BindModels
{
  public class CreatePlaceBindModel
  {
    public int placeTypeId { get; set; }
    public string placeName { get; set; }
    public string longitude { get; set; }
    public string latitude { get; set; }
  }

  public class MapPlaceBindModel
  {
    public int placeTypeId { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
  }

  public class GetPlacesByDistanceBindModel
  {
    public string longitude { get; set; }
    public string latitude { get; set; }
  }

  public class PlaceBindModel
  {
    public int id { get; set; }
    public string name { get; set; }
    public int placeTypeId { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
    public string creationDate { get; set; }
    public double distance { get; set; }
  }
}