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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Hello World!");

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
            return Results.StatusCode(500);
    }
}
);

app.MapGet("/traveller/{id}", (int id) => "Traveller endpoint placeholder");

app.MapGet("/traveller/search", (
    [FromQuery] int? flightId,
    [FromQuery] string? name,
    [FromQuery(Name = "Dob_to")] string? dobTo,
    [FromQuery(Name = "Dob_from")] string? dobFrom
) =>
{
    var request = new TravellerQueryRequest
    {
        FlightId = flightId,
        Name = name,
        Dob_to = dobTo,
        Dob_from = dobFrom
    };
    return $" search endpoint {request}";
}
);


app.Run();
