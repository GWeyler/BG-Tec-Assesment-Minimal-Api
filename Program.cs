using Serilog;
using Mapster;
using BG_Tec_Assesment_Minimal_Api.Data;
using Microsoft.EntityFrameworkCore;
using BG_Tec_Assesment_Minimal_Api.DTO;
using Microsoft.AspNetCore.Mvc;
using BG_Tec_Assesment_Minimal_Api.Services;
using BG_Tec_Assesment_Minimal_Api.Utils;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data.Entity.Core.Mapping;
using BG_Tec_Assesment_Minimal_Api.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<TravellerAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMapster();

builder.Services.AddOpenApi();

builder.Services.AddScoped < ITravellerService, TravellerService >();

builder.Services.AddScoped(typeof(IGenericRepository<Traveller>), typeof(TravellerRepository));

builder.Services.AddScoped(typeof(IGenericRepository<Flight>), typeof(FlightRepository));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/check-in", async ([FromBody] CheckInRequest request, ITravellerService travellerService) =>
{
    var ret = await travellerService.CheckInTravellerAsync(request);
    switch (ret.ErrorCode)
    {
        case ErrorEnum.None:
            return Results.Ok(new { status = ret.Message, travellerId = ret.TravellerId });
        case ErrorEnum.NotFound:
            return Results.NotFound(new { status = ret.Message });
        case ErrorEnum.InternalServerError:
            return Results.InternalServerError();
        case ErrorEnum.BadRequest:
            return Results.BadRequest(new { status = ret.Message });
        case ErrorEnum.DuplicateEntry:
            return Results.BadRequest(new { status = "Duplicate", reason = ret.Message });
        default:
            return Results.InternalServerError();
    }
}
);

app.MapGet("/traveller/{id}", async (int id, ITravellerService travellerService) =>
{
    var ret = await travellerService.GetTravellerByIdAsync(id);

    switch (ret.ErrorCode)
    {
        case ErrorEnum.None:
            return Results.Ok(ret.Traveller);
        case ErrorEnum.NotFound:
            return Results.NotFound();
        default:
            return Results.InternalServerError();
    }

});

app.MapGet("/traveller/search", async  (
    [FromQuery] int? flightId,
    [FromQuery] string? name,
    [FromQuery(Name = "dob-to")] string? dobTo,
    [FromQuery(Name = "dob-from")] string? dobFrom,
    ITravellerService travellerService
) =>
{
    var request = new TravellerSearchRequest
    {
        FlightId = flightId,
        Name = name,
        Dob_to = dobTo,
        Dob_from = dobFrom
    };

    var ret = await travellerService.SearchTravellerAsync(request);

    switch (ret.ErrorCode)
    {
        case ErrorEnum.None:
            return Results.Ok(new { result = ret.Travellers });
        case ErrorEnum.BadRequest:
            return Results.BadRequest(ret.Messsage);
        default:
            return Results.InternalServerError();
    }
}
);


app.Run();
