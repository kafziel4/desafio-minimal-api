using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Endpoints;

public static class HomeEndpoints
{
    public static WebApplication MapHomeEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Json(new Home()))
            .WithTags("Home")
            .Produces<Home>(StatusCodes.Status200OK);

        return app;
    }
}
