using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using Api.BindModels;
using Api.Data.Entities;

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
        }
    }
}