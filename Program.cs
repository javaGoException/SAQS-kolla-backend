using API;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Endpoints.Map(app);

app.Run();
