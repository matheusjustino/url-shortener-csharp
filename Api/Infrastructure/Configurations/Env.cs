namespace url_shortener.Api.Infrastructure.Configurations;

public static class Env
{
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}
