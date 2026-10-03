using Mt.Core;
using Mt.Tests.Base;
using Mt.Tests.Environments;

namespace Mt.Tests;

public class DeviceInfoTest : TestBase
{
    private DeviceEnvironment DeviceEnvironment => Resolve<DeviceEnvironment>();

    [Test]
    public void DeviceInfo()
    {
        var device = DeviceEnvironment.Setup(d => d with
        {
            Vendor = "Google", 
            Product = "Pixel 8"
        });
        DeviceEnvironment.SetupInfo(device, i => i with
        {
            SerialNumber = "ABC123",
            FriendlyName = "My Pixel",
            Storages =
            [
                new DeviceStorage
                {
                    Description = "Internal shared storage",
                    Capacity = 128_000_000_000,
                    FreeSpace = 64_000_000_000,
                },
            ],
        });

        var (exitCode, output) = Run("-i", "0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(output, Does.Contain("Manufacturer:  Google"));
            Assert.That(output, Does.Contain("Model:         Pixel 8"));
            Assert.That(output, Does.Contain("Serial number: ABC123"));
            Assert.That(output, Does.Contain("Version:       -"));
            Assert.That(output, Does.Contain("Name:          My Pixel"));
            Assert.That(output, Does.Contain("Internal shared storage: 64.0 GB free of 128.0 GB"));
        }
    }

    [Test]
    public void DeviceInfoNotFound()
    {
        var (exitCode, output) = Run("-i", "0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(1));
            Assert.That(output, Is.Empty);
        }
    }

    [TestCase("abc")]
    [TestCase("1.5")]
    public void DeviceInfoInvalidNumber(string number)
    {
        var device = DeviceEnvironment.Setup();
        DeviceEnvironment.SetupInfo(device);

        var (exitCode, output) = Run("-i", number);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exitCode, Is.EqualTo(1));
            Assert.That(output, Is.Empty);
        }
    }
}
