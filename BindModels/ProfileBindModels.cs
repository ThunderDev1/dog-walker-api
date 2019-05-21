namespace Api.BindModels
{
  public class ProfileBindModel
  {
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string email { get; set; }
    public string avatarUrl { get; set; }
  }

  public class PublicProfileBindModel
  {
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string avatarUrl { get; set; }
  }

  public class CreateProfileBindModel
  {
    public string email { get; set; }
  }

  public class ProfileUpdateBindModel
  {
    public string name { get; set; }
    public string description { get; set; }
  }
}