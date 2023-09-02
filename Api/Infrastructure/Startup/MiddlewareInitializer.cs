namespace url_shortener.Api.Infrastructure.Startup;

using url_shortener.Api.Infrastructure.Extensions;
using url_shortener.Api.Infrastructure.Middleware;

public static partial class MiddlewareInitializer
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        ConfigureSwagger(app);

        app.UseCors("CorsPolicy");
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.ApplyMigrations();

        return app;
    }

    private static void ConfigureSwagger(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
