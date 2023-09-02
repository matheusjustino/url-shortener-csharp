namespace url_shortener.Api.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using url_shortener.Api.Infrastructure.Persistence;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}