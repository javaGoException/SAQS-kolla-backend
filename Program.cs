using API;
using Application.Services;
using Infrastructure.Services;
using Infrastructure.Setup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IObjectiveService, ObjectiveService>();

builder.Services.AddScoped<IDatabaseManager, SqliteManager>();
builder.Services.AddSingleton<SqliteInitializer>();

var app = builder.Build();
ObjectiveEndpoints.Map(app);

using (var scope = app.Services.CreateScope())
{
    SqliteInitializer sqliteInitializer = scope.ServiceProvider.GetRequiredService<SqliteInitializer>();
    await sqliteInitializer.InitializeDatabase();
}

app.Run();
