using Mt.Tests.Base;
using Mt.Tests.Environments;

namespace Mt.Tests;

public class ListDevicesTest : TestBase
{
    private DeviceEnvironment DeviceEnvironment => Resolve<DeviceEnvironment>();

    [Test]
    public void ListDevices()
    {
        DeviceEnvironment.Setup(d => d with { Vendor = "Google", Product = "Pixel 8" });
        DeviceEnvironment.Setup(d => d with { Vendor = "Amazon", Product = "Kindle Scribe" });

        var (exitCode, output) = Run("-l");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(output, Is.EqualTo(
                "0 - Google Pixel 8" + Environment.NewLine +
                "1 - Amazon Kindle Scribe" + Environment.NewLine));
        }
    }

    [Test]
    public void ListNoDevices()
    {
        var (exitCode, output) = Run("-l");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(output, Does.StartWith("No devices found."));
        }
    }

    [Test]
    public void ListDevicesError()
    {
        DeviceEnvironment.Error = new Exception("Device detection failed: UsbLayer");

        var (exitCode, output) = Run("-l");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(1));
            Assert.That(output, Is.Empty);
        }
    }
}
