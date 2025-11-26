using Serilog;
using Mapster;
using BG_Tec_Assesment_Minimal_Api.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<TravellerAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMapster();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/checkin", () => "Check -in endpoint placeholder");

app.MapGet("/traveller/{id}", (int id) => "Traveller endpoint placeholder");

app.MapGet("/traveller/search", () => " search endpoint ");


app.Run();
