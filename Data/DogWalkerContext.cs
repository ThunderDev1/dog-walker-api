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
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<UserMeeting> UserMeetings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // prevent pluralization of table names
      modelBuilder.Entity<User>().ToTable("User");
      modelBuilder.Entity<Place>().ToTable("Place");
      modelBuilder.Entity<PlaceType>().ToTable("PlaceType");
      modelBuilder.Entity<Meeting>().ToTable("Meeting");
      modelBuilder.Entity<UserMeeting>().ToTable("UserMeeting");

      modelBuilder.Entity<UserMeeting>()
                .HasOne(p => p.User)
                .WithMany(u => u.UserMeetings)
                .HasForeignKey(p => p.UserId)
                .HasConstraintName("ForeignKey_UserMeeting_User")
                .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<UserMeeting>()
          .HasOne(p => p.Meeting)
          .WithMany(u => u.UserMeetings)
          .HasForeignKey(p => p.MeetingId);

      modelBuilder.Entity<UserMeeting>()
          .HasKey(um => new { um.UserId, um.MeetingId });
    }
  }
}