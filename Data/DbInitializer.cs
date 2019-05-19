using System;
using System.Collections.Generic;
using System.Linq;
using Api.Data.Entities;

namespace Api.Data
{
    public static class DbInitializer
    {
        public static void Seed(DogWalkerContext dbContext)
        {
            if (!dbContext.PlaceTypes.Any())
            {
                var placeTypes = new List<PlaceType>();

                placeTypes.Add(new PlaceType { Id = 1, Name = "WasteBag" });
                placeTypes.Add(new PlaceType { Id = 2, Name = "Park" });

                dbContext.PlaceTypes.AddRange(placeTypes);
            }
        }
    }
}