namespace url_shortener.Api.Infrastructure.Configurations;

using System.Reflection;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, string serviceNamespace, Assembly assembly)
    {
        var serviceTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.Namespace == serviceNamespace);

        foreach (var serviceType in serviceTypes)
        {
            var implementedInterface = serviceType.GetInterface($"I{serviceType.Name}");
            if (implementedInterface is not null)
            {
                services.AddScoped(implementedInterface, serviceType);
            }
        }
    }
}