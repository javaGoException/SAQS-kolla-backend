namespace API;

public static class Endpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/", async context =>
        {
            await context.Response.WriteAsJsonAsync(new { Message = "Testing" });
        });
    }
}