using Mt.Core;

namespace Mt.Tests.Environments;

public class AppEnvironment
{
    public App? App { get; private set; }

    public App Setup(Func<App, App>? configure = null)
    {
        var app = new App
        {
            Name = "mt",
            Version = "0.1.0",
        };
        App = configure?.Invoke(app) ?? app;
        return App;
    }
}
