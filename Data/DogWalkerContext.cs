using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Api.Data.Entities;

namespace Api.Data
{
  public class DogWalkerContext : DbContext
  {
    public DogWalkerContext(DbContextOptions<DogWalkerContext> options)
        : base(options)
    { }

    public DbSet<User> Users { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<PlaceType> PlaceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // prevent pluralization of table names
      modelBuilder.Entity<User>().ToTable("User");
      modelBuilder.Entity<Place>().ToTable("Place");
      modelBuilder.Entity<PlaceType>().ToTable("PlaceType");
    }
  }
}