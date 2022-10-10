using Microsoft.Extensions.DependencyInjection;
using Sigger.Registry;

namespace Sigger;

public static class StartupExtensions
{
    /// <summary>
    /// Add the UserRepository to the services collection
    /// </summary>
    public static IServiceCollection AddSiggerRepository(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ISiggerRepository<>), typeof(SiggerRepository<>));
        services.AddSingleton(typeof(ISiggerRepository<,>), typeof(SiggerRepository<,>));
        return services;
    }

}