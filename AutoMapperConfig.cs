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

      CreateMap<User, UserModel>();
      CreateMap<Place, PlaceModel>()
        .ForMember(p => p.Longitude, opts => opts.MapFrom(src => src.Location.X))
        .ForMember(p => p.Latitude, opts => opts.MapFrom(src => src.Location.Y));

      CreateMap<GuestModel, GuestBindModel>()
        .ForMember(p => p.id, opts => opts.MapFrom(src => src.Id))
        .ForMember(p => p.avatarUrl, opts => opts.MapFrom(src => src.AvatarUrl))
        .ForMember(p => p.name, opts => opts.MapFrom(src => src.Name))
        .ForMember(p => p.description, opts => opts.MapFrom(src => src.Description))
        .ForMember(p => p.status, opts => opts.MapFrom(src => src.Status));
    }
  }
}