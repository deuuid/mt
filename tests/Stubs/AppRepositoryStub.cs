using Mt.Core;
using Mt.Tests.Environments;

namespace Mt.Tests.Stubs;

public class AppRepositoryStub(AppEnvironment environment) : IAppRepository
{
    public App Get() => environment.App ?? environment.Setup();
}
