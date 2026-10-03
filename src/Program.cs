using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Mt.Pres;
using Mt.Pres.Commands;
using Mt.Core;
using Mt.Data;

namespace Mt;

public static class Program
{
    internal static readonly IReadOnlyDictionary<string, Type> Commands = new Dictionary<string, Type>
    {
        { "-h", typeof(HelpCommand) },
        { "-v", typeof(VersionCommand) },
        { "-l", typeof(ListDevicesCommand) },
        { "-i", typeof(DeviceInfoCommand) }
    };

    public static int Main(string[] args)
    {
        using var services = Configure().BuildServiceProvider();
        return Run(args, services);
    }

    internal static int Run(string[] args, IServiceProvider services)
    {
        var culture = new CultureInfo("en-US");
        CultureInfo.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;

        var output = services.GetRequiredService<OutputService>();

        try
        {
            var commandName = args.FirstOrDefault() ?? "-h";
            if (!Commands.TryGetValue(commandName, out var commandType))
                return output.Fail($"Unknown command: {commandName}{Environment.NewLine}Run 'mt -h' for usage.");

            return ((ICommand)services.GetRequiredService(commandType)).Run(args);
        }
        catch (Exception e)
        {
            return output.Fail(services.GetRequiredService<ExceptionHandler>().Handle(e));
        }
    }

    private static IServiceCollection Configure()
    {
        var services = new ServiceCollection()
            .AddSingleton<IAppRepository, AppRepository>()
            .AddSingleton<IDeviceRepository, DeviceRepository>()
            .AddSingleton<ExceptionHandler>()
            .AddSingleton<OutputService>();

        foreach (var command in Commands.Values)
            services.AddSingleton(command);

        return services;
    }
}
