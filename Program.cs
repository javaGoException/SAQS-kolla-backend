using API;
using Application.Services;
using Infrastructure.Services;
using Infrastructure.Setup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IObjectiveService, ObjectiveService>();

builder.Services.AddScoped<IDatabaseManager, SqliteManager>();
builder.Services.AddScoped<IDatabaseConnector, SqliteConnector>();

var app = builder.Build();

ObjectiveEndpoints.Map(app);

app.Run();
