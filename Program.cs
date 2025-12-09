using SAQS_kolla_backend.API;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Application.Services;
using SAQS_kolla_backend.Infrastructure.Services;
using SAQS_kolla_backend.Infrastructure.Setup;
using SAQS_kolla_backend.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IObjectiveService, ObjectiveService>();

builder.Services.AddScoped<IObjectiveRepository, ObjectiveRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IDatabaseConnector, SqliteConnector>();
builder.Services.AddSingleton<SqliteInitializer>();
builder.Services.AddSingleton<ValidatorService>();

builder.Services.AddOptions<DatabaseOptions>()
    .BindConfiguration("DatabaseOptions")
    .Validate(opts => opts.Validate())
    .ValidateOnStart();

var app = builder.Build();
ObjectiveEndpoints.Map(app);
RoleEndpoints.Map(app);

using (var scope = app.Services.CreateScope())
{
    SqliteInitializer sqliteInitializer = scope.ServiceProvider.GetRequiredService<SqliteInitializer>();
    await sqliteInitializer.InitializeDatabase();
}

app.Run();
