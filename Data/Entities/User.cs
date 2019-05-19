using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Api.Data.Entities
{
  public class User
  {
    public string Id { get; set; }
    public string AvatarUrl { get; set; }
  }
}