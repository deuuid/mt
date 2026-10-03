using Mt.Tests.Base;
using Mt.Tests.Environments;

namespace Mt.Tests;

public class ExceptionTest : TestBase
{
    private DeviceEnvironment DeviceEnvironment => Resolve<DeviceEnvironment>();

    [Test]
    public void UnknownArgument()
    {
        var (exitCode, output) = Run("-x");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(1));
            Assert.That(output, Is.Empty);
        }
    }

    [Test]
    public void LibraryNotLoaded()
    {
        DeviceEnvironment.Error = new DllNotFoundException("Unable to load shared library 'mtp'");

        var (exitCode, output) = Run("-l");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(1));
            Assert.That(output, Is.Empty);
        }
    }
}
