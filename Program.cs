using API;
using Application.Services;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IObjectiveService, ObjectiveService>();
builder.Services.AddScoped<IDatabaseManager, DatabaseManager>();

var app = builder.Build();

ObjectiveEndpoints.Map(app);

app.Run();
