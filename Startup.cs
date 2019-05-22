using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Settings;
using Api.Services;
using AutoMapper;

namespace Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvcCore(config =>
      {
        var policy = new AuthorizationPolicyBuilder()
                   .RequireAuthenticatedUser()
                   .Build();
        config.Filters.Add(new AuthorizeFilter(policy)); //global authorize filter
      })
          .AddAuthorization()
          .AddJsonFormatters();

      var endpointsSection = Configuration.GetSection("Endpoints");
      var endpoints = endpointsSection.Get<EndpointSettings>();
      services.Configure<StorageSettings>(Configuration.GetSection("Storage"));

      services.AddAuthentication("Bearer")
          .AddIdentityServerAuthentication(options =>
          {
            options.Authority = endpoints.Identity;
            options.RequireHttpsMetadata = false;
            options.ApiName = "dwk-api";
          });

      services.AddCors(options =>
      {
        options.AddPolicy("default", policy =>
        {
          policy.WithOrigins(endpoints.Spa)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
      });

      services.AddDbContext<DogWalkerContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
          x => x.UseNetTopologySuite()) // enable mapping to spatial types
          );

      services.AddTransient<IPlaceService, PlaceService>();
      services.AddTransient<IProfileService, ProfileService>();
      services.AddTransient<IFileService, FileService>();
      services.AddTransient<IMeetingService, MeetingService>();
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {
        var dbContext = serviceScope.ServiceProvider.GetService<DogWalkerContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
        {
          dbContext.Database.Migrate();
          DbInitializer.Seed(dbContext);
        }
      }

      app.UseCors("default");
      app.UseAuthentication();
      app.UseMvc();
    }
  }
}
