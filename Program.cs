using Domain.DTO;
using Infra.Db;
using Microsoft.EntityFrameworkCore;


#region Builder
var builder = WebApplication.CreateBuilder(args);


#region DbContext
var connectionString = builder.Configuration.GetConnectionString("mysql");

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
#endregion


var app = builder.Build();
#endregion





app.MapGet("/", () => "Hello World!");



app.MapPost("/Login", (LoginDto loginDto) =>
{
    return loginDto = null!;   
});

app.Run();