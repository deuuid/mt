using Mt.Tests.Base;
using Mt.Tests.Environments;

namespace Mt.Tests;

public class VersionTest : TestBase
{
    private AppEnvironment AppEnvironment => Resolve<AppEnvironment>();

    [Test]
    public void Print()
    {
        var (exitCode, output) = Run("-v");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(output, Is.EqualTo("mt 0.1.0" + Environment.NewLine));
        }
    }

    [Test]
    public void PrintWithoutName()
    {
        AppEnvironment.Setup(a => a with { Name = null });

        var (exitCode, output) = Run("-v");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(output, Is.EqualTo("- 0.1.0" + Environment.NewLine));
        }
    }

    [Test]
    public void PrintWithoutVersion()
    {
        AppEnvironment.Setup(a => a with { Version = null });

        var (exitCode, output) = Run("-v");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(output, Is.EqualTo("mt -" + Environment.NewLine));
        }
    }
}
