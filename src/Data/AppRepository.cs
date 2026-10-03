using System.Reflection;
using Mt.Core;

namespace Mt.Data;

public class AppRepository : IAppRepository
{
    public App Get()
    {
        var assembly = typeof(AppRepository).Assembly;
        return new App
        {
            Name = assembly.GetName().Name,
            Version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
        };
    }
}
