using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using Api.BindModels;
using Api.Data.Entities;
using Api.Models;

namespace Api
{
  public class ApiProfile : Profile
  {
    public ApiProfile()
    {
      CreateMap<CreatePlaceBindModel, Place>();
      CreateMap<ProfileBindModel, User>();
      CreateMap<User, ProfileBindModel>();
      CreateMap<User, PublicProfileBindModel>();

      CreateMap<PlaceModel, PlaceBindModel>()
        .ForMember(p => p.creationDate, opts => opts.MapFrom(src => src.CreationDate.ToString("o", CultureInfo.InvariantCulture)));
    }
  }
}