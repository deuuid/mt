using Microsoft.Extensions.DependencyInjection;
using Mt.Core;
using Mt.Pres;
using Mt.Tests.Environments;
using Mt.Tests.Stubs;

namespace Mt.Tests.Base;

public abstract class TestBase
{
    private ServiceProvider _services = null!;

    [SetUp]
    public void SetUp()
    {
        _services = Configure().BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _services.Dispose();
    }

    private static IServiceCollection Configure()
    {
        var services = new ServiceCollection()
            .AddSingleton<AppEnvironment>()
            .AddSingleton<IAppRepository, AppRepositoryStub>()
            .AddSingleton<DeviceEnvironment>()
            .AddSingleton<IDeviceRepository, DeviceRepositoryStub>()
            .AddSingleton<ExceptionHandler>()
            .AddSingleton<OutputService>();

        foreach (var command in Program.Commands.Values)
            services.AddSingleton(command);

        return services;
    }

    protected T Resolve<T>() where T : notnull => _services.GetRequiredService<T>();

    protected (int ExitCode, string Output) Run(params string[] args)
    {
        var originalOut = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);
        try
        {
            var exitCode = Program.Run(args, _services);
            return (exitCode, output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
