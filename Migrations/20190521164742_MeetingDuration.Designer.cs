﻿// <auto-generated />
using System;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace Api.Migrations
{
    [DbContext(typeof(DogWalkerContext))]
    [Migration("20190521164742_MeetingDuration")]
    partial class MeetingDuration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Api.Data.Entities.Meeting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate");

                    b.Property<TimeSpan>("Duration");

                    b.Property<int>("PlaceId");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Title");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.HasIndex("UserId");

                    b.ToTable("Meeting");
                });

            modelBuilder.Entity("Api.Data.Entities.Place", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate");

                    b.Property<Point>("Location")
                        .HasColumnType("geometry");

                    b.Property<string>("Name");

                    b.Property<int>("PlaceTypeId");

                    b.HasKey("Id");

                    b.HasIndex("PlaceTypeId");

                    b.ToTable("Place");
                });

            modelBuilder.Entity("Api.Data.Entities.PlaceType", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("PlaceType");
                });

            modelBuilder.Entity("Api.Data.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AvatarUrl");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Api.Data.Entities.UserMeeting", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("MeetingId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime>("ModificationDate");

                    b.Property<int>("Status");

                    b.HasKey("UserId", "MeetingId");

                    b.HasIndex("MeetingId");

                    b.ToTable("UserMeeting");
                });

            modelBuilder.Entity("Api.Data.Entities.Meeting", b =>
                {
                    b.HasOne("Api.Data.Entities.Place", "Place")
                        .WithMany()
                        .HasForeignKey("PlaceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Data.Entities.User", "User")
                        .WithMany("Meetings")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Api.Data.Entities.Place", b =>
                {
                    b.HasOne("Api.Data.Entities.PlaceType", "PlaceType")
                        .WithMany("Places")
                        .HasForeignKey("PlaceTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Data.Entities.UserMeeting", b =>
                {
                    b.HasOne("Api.Data.Entities.Meeting", "Meeting")
                        .WithMany("UserMeetings")
                        .HasForeignKey("MeetingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Data.Entities.User", "User")
                        .WithMany("UserMeetings")
                        .HasForeignKey("UserId")
                        .HasConstraintName("ForeignKey_UserMeeting_User")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
