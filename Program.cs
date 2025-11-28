using BG_Tec_Assesment_Minimal_Api.Data;
using BG_Tec_Assesment_Minimal_Api.DTO;
using BG_Tec_Assesment_Minimal_Api.Models;
using BG_Tec_Assesment_Minimal_Api.Services;
using BG_Tec_Assesment_Minimal_Api.Utils;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Data.Entity.Core.Mapping;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context,loggetConfig) =>
    loggetConfig.ReadFrom.Configuration(context.Configuration));


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

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSerilogRequestLogging();

app.MapPost("/check-in", async ([FromBody] CheckInRequest request, ITravellerService travellerService, HttpContext context) =>
{
    
    var ret = await travellerService.CheckInTravellerAsync(request);
    switch (ret.ErrorCode)
    {
        case ErrorEnum.None:
            return Results.Ok(new { ret.Value });
        case ErrorEnum.NotFound:
            return Results.NotFound(new { status = ret.Message });
        case ErrorEnum.InternalServerError:
            return Results.Problem(
                type: "Internal Server Error",
                statusCode: StatusCodes.Status500InternalServerError,
                detail: ret.Message);
        case ErrorEnum.BadRequest:
            return Results.Problem(
                 type: "Bad Request",
                 statusCode: StatusCodes.Status400BadRequest,
                 detail: ret.Message);
        case ErrorEnum.DuplicateEntry:
            return Results.Ok(new { status = ret.Value.Status, reason = ret.Message });
        default:
            return Results.Problem(
                type: "Internal Server Error",
                statusCode: StatusCodes.Status500InternalServerError,
                detail: ret.Message == null ? "Unkown": ret.Message);
    }
}
);

app.MapGet("/traveller/{id}", async (int id, ITravellerService travellerService, HttpContext context) =>
{
    var ret = await travellerService.GetTravellerByIdAsync(id);

    switch (ret.ErrorCode)
    {
        case ErrorEnum.None:
            return Results.Ok(ret.Value);
        case ErrorEnum.NotFound:
            return Results.NotFound(new {status="No Traverller found"});
        default:
            return Results.Problem(
                type: "Internal Server Error",
                statusCode: StatusCodes.Status500InternalServerError,
                detail: ret.Message );
    }
});

app.MapGet("/traveller/search", async  (
    [FromQuery] int? flightId,
    [FromQuery] string? name,
    [FromQuery(Name = "dob-to")] string? dobTo,
    [FromQuery(Name = "dob-from")] string? dobFrom,
    ITravellerService travellerService,
    HttpContext context
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
            return Results.Ok(new { result = ret.Value});
        case ErrorEnum.BadRequest:
            return Results.Problem(
                type: "Bad Request",
                statusCode: StatusCodes.Status400BadRequest,
                detail: ret.Message);
        default:
            return Results.Problem(
                type: "Internal Server Error",
                statusCode: StatusCodes.Status500InternalServerError,
                detail: ret.Message);
    }
}
);


app.Run();
