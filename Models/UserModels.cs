using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Api.Models
{
  public class UserModel
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public string PushToken { get; set; }
    public DateTime CreationDate { get; set; }
  }
}