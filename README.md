# dog-walker-api

Dotnet core 2.2 API that goes with the [dog-walker](https://github.com/ThunderDev1/dog-walker) project

The API has the following features:

- OpenId authentication with [IdentityServer](https://identityserver.io)
- Web notifications using [FCM](https://firebase.google.com/docs/cloud-messaging/)
- [Spatial Data](https://docs.microsoft.com/en-us/ef/core/modeling/spatial) querying

## Using this project

You will need to setup a few things:

- Install [SQL Server LocalDB](https://go.microsoft.com/fwlink/?linkid=853017)
- Setup identity server: an example can be found in one of my other repos [reactjs-ts-identityserver](https://github.com/ThunderDev1/reactjs-ts-identityserver)
- Create a firebase account and add your key to FirebaseServerKey entry in appsettings.json

`dotnet ef database update`

`donet run`

## Stack

* [Dotnet core 2.2](https://www.microsoft.com/net/core#windowscmd) modular and high performance implementation of .NET
* [SQL Server](https://www.microsoft.com/sql-server) relational database management system developed by Microsoft
* [EF Core 2.2](https://docs.microsoft.com/ef/core/) lightweight version of Entity Framework ORM
* [IdentityServer4](https://github.com/identityserver) OpenID Connect Provider and OAuth 2.0 Authorization Server